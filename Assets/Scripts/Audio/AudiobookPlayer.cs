using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Core audiobook logic: file discovery, loading, playback, chapter parsing,
/// and user bookmark persistence.
///
/// Does NOT touch any Unity UI — AudiobookUI.cs drives this via events.
///
/// FILE PAIRING (all files live in StreamingAssets/Audiobook/):
///   mybook.mp3                  ← the audio
///   mybook.chapters.txt         ← authored chapters, read-only in-app (you write this)
///   mybook.bookmarks.txt        ← user bookmarks, auto-saved by this script
///
/// CHAPTERS FILE FORMAT (CSV, one entry per line):
///   # comment lines are ignored
///   Prologue,0
///   Chapter 1 - The Road,125.5
///
/// BOOKMARKS FILE FORMAT (auto-generated, safe to hand-edit):
///   # User bookmarks
///   My note,234.5
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudiobookPlayer : MonoBehaviour
{
    // ─── Constants ────────────────────────────────────────────────────────────

    private const string AudiobookFolder = "Audiobook";
    private static readonly string[] SupportedExtensions = { ".mp3", ".wav", ".ogg" };

    // ─── Events ───────────────────────────────────────────────────────────────

    /// <summary>A fresh list of full file paths discovered in StreamingAssets/Audiobook/.</summary>
    public event Action<List<string>> OnFilesDiscovered;

    /// <summary>Fired when a file has loaded successfully — passes the display name.</summary>
    public event Action<string> OnFileLoaded;

    /// <summary>Fired when file loading fails.</summary>
    public event Action OnFileLoadFailed;

    /// <summary>Fired when chapters for the current file are ready.</summary>
    public event Action<List<BookmarkEntry>> OnChaptersLoaded;

    /// <summary>Fired whenever the user bookmark list changes (add / remove / initial load).</summary>
    public event Action<List<BookmarkEntry>> OnBookmarksChanged;

    /// <summary>Fired when playback state changes.</summary>
    public event Action<PlaybackState> OnPlaybackStateChanged;

    /// <summary>Fired ~4 times per second during playback. Args: currentSeconds, totalSeconds.</summary>
    public event Action<float, float> OnPlaybackProgressChanged;

    // ─── Types ────────────────────────────────────────────────────────────────

    public enum PlaybackState { Idle, Loading, Playing, Paused, Stopped }

    [Serializable]
    public class BookmarkEntry
    {
        public string Label;
        public float  Timestamp;
        public bool   IsUserBookmark; // false = authored chapter (cannot delete)

        public BookmarkEntry(string label, float timestamp, bool isUserBookmark)
        {
            Label         = label;
            Timestamp     = timestamp;
            IsUserBookmark = isUserBookmark;
        }
    }

    // ─── Public State ─────────────────────────────────────────────────────────

    public PlaybackState CurrentState    { get; private set; } = PlaybackState.Idle;
    public string        CurrentFileName { get; private set; }
    public float         CurrentTime     => _src != null ? _src.time : 0f;
    public float         TotalTime       => _src != null && _src.clip != null ? _src.clip.length : 0f;

    // ─── Inspector ────────────────────────────────────────────────────────────

    [Header("Progress Update Rate")]
    [Tooltip("How often (seconds) the progress event fires during playback.")]
    [SerializeField] private float progressUpdateInterval = 0.25f;

    // ─── Private ──────────────────────────────────────────────────────────────

    private AudioSource          _src;
    private List<string>         _availableFiles  = new();
    private List<BookmarkEntry>  _chapters        = new();
    private List<BookmarkEntry>  _userBookmarks   = new();
    private string               _bookmarksPath;
    private Coroutine            _progressRoutine;

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    private void Awake()
    {
        _src = GetComponent<AudioSource>();
        _src.spatialBlend = 0f;
        _src.loop         = false;
        _src.playOnAwake  = false;
        DiscoverFiles();
    }

    // ─── Convenience ─────────────────────────────────────────────────────────

    /// <summary>
    /// Assigns an AudioClip directly (no file loading) and begins playback.
    /// Use this instead of LoadAndPlay when the clip is a serialized asset reference.
    /// Pass a TextAsset for the matching .chapters.txt if you have one.
    /// </summary>
    public void LoadClip(AudioClip clip, TextAsset chaptersAsset = null)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudiobookPlayer] LoadClip called with null clip.");
            return;
        }

        if (_progressRoutine != null) StopCoroutine(_progressRoutine);

        CurrentFileName = clip.name;
        _src.clip = clip;
        _src.Stop();

        SetState(PlaybackState.Stopped);
        OnFileLoaded?.Invoke(CurrentFileName);

        if (chaptersAsset != null)
            LoadChaptersFromText(chaptersAsset.text);
        else
            OnChaptersLoaded?.Invoke(new List<BookmarkEntry>());

        LoadUserBookmarksForFile(clip.name);
        Play();
    }

    /// <summary>
    /// Finds the file matching fileNameWithoutExtension (case-insensitive),
    /// loads it, and starts playback automatically.
    /// </summary>
    public void LoadAndPlay(string fileNameWithoutExtension)
    {
        string match = _availableFiles.Find(f =>
            string.Equals(Path.GetFileNameWithoutExtension(f), fileNameWithoutExtension,
                          StringComparison.OrdinalIgnoreCase));

        if (match == null)
        {
            Debug.LogWarning($"[AudiobookPlayer] File not found: '{fileNameWithoutExtension}'");
            return;
        }

        StartCoroutine(LoadThenPlay(match));
    }

    private IEnumerator LoadThenPlay(string fullPath)
    {
        LoadFile(fullPath);
        while (CurrentState == PlaybackState.Loading)
            yield return null;
        if (CurrentState == PlaybackState.Stopped)
            Play();
    }

    // ─── File Discovery ───────────────────────────────────────────────────────

    /// <summary>Scans StreamingAssets/Audiobook/ for supported audio files and fires OnFilesDiscovered.</summary>
    public void DiscoverFiles()
    {
        _availableFiles.Clear();
        string folder = Path.Combine(Application.streamingAssetsPath, AudiobookFolder);

        if (!Directory.Exists(folder))
        {
            Debug.Log($"[AudiobookPlayer] Creating audiobook folder at: {folder}");
            Directory.CreateDirectory(folder);
        }

        foreach (string ext in SupportedExtensions)
        {
            foreach (string f in Directory.GetFiles(folder, $"*{ext}", SearchOption.TopDirectoryOnly))
                _availableFiles.Add(f);
        }

        _availableFiles.Sort(StringComparer.OrdinalIgnoreCase);
        OnFilesDiscovered?.Invoke(new List<string>(_availableFiles));
    }

    // ─── File Loading ─────────────────────────────────────────────────────────

    /// <summary>Loads the audio file at fullPath and its companion chapter/bookmark files.</summary>
    public void LoadFile(string fullPath)
    {
        if (_progressRoutine != null) StopCoroutine(_progressRoutine);
        StartCoroutine(LoadRoutine(fullPath));
    }

    private IEnumerator LoadRoutine(string fullPath)
    {
        SetState(PlaybackState.Loading);
        CurrentFileName = Path.GetFileNameWithoutExtension(fullPath);

        // Unity requires forward slashes and file:// prefix
        string uri = "file:///" + fullPath.Replace('\\', '/');
        AudioType audioType = ResolveAudioType(fullPath);

        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, audioType);
        ((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[AudiobookPlayer] Failed to load '{fullPath}': {www.error}");
            SetState(PlaybackState.Idle);
            OnFileLoadFailed?.Invoke();
            yield break;
        }

        _src.clip = DownloadHandlerAudioClip.GetContent(www);
        _src.Stop();

        SetState(PlaybackState.Stopped);
        OnFileLoaded?.Invoke(CurrentFileName);

        // Load companion files
        LoadChapters(fullPath);
        LoadUserBookmarks(fullPath);
    }

    private static AudioType ResolveAudioType(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".mp3" => AudioType.MPEG,
            ".ogg" => AudioType.OGGVORBIS,
            ".wav" => AudioType.WAV,
            _      => AudioType.UNKNOWN
        };
    }

    // ─── Chapter Loading ──────────────────────────────────────────────────────

    private void LoadChapters(string audioFilePath)
    {
        _chapters.Clear();
        string chaptersPath = StripExtension(audioFilePath) + ".chapters.txt";

        if (!File.Exists(chaptersPath))
        {
            Debug.Log($"[AudiobookPlayer] No chapters file at: {chaptersPath}  (create one to add chapters)");
            OnChaptersLoaded?.Invoke(new List<BookmarkEntry>(_chapters));
            return;
        }

        LoadChaptersFromText(File.ReadAllText(chaptersPath));
    }

    private void LoadChaptersFromText(string text)
    {
        _chapters.Clear();
        foreach (string line in text.Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) continue;
            string[] parts = line.Split(',');
            if (parts.Length >= 2 &&
                float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float ts))
            {
                _chapters.Add(new BookmarkEntry(parts[0].Trim(), ts, isUserBookmark: false));
            }
        }
        OnChaptersLoaded?.Invoke(new List<BookmarkEntry>(_chapters));
    }

    // ─── User Bookmarks ───────────────────────────────────────────────────────

    // Used by LoadClip (serialized asset path). Bookmarks stored in persistentDataPath
    // so this works on all platforms including Android.
    private void LoadUserBookmarksForFile(string clipName)
    {
        _userBookmarks.Clear();
        _bookmarksPath = Path.Combine(Application.persistentDataPath, clipName + ".bookmarks.txt");

        if (File.Exists(_bookmarksPath))
        {
            foreach (string line in File.ReadAllLines(_bookmarksPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) continue;
                string[] parts = line.Split(',');
                if (parts.Length >= 2 &&
                    float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float ts))
                {
                    _userBookmarks.Add(new BookmarkEntry(parts[0].Trim(), ts, isUserBookmark: true));
                }
            }
        }

        OnBookmarksChanged?.Invoke(new List<BookmarkEntry>(_userBookmarks));
    }

    private void LoadUserBookmarks(string audioFilePath)
    {
        _userBookmarks.Clear();
        _bookmarksPath = StripExtension(audioFilePath) + ".bookmarks.txt";

        if (File.Exists(_bookmarksPath))
        {
            foreach (string line in File.ReadAllLines(_bookmarksPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) continue;
                string[] parts = line.Split(',');
                if (parts.Length >= 2 &&
                    float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float ts))
                {
                    _userBookmarks.Add(new BookmarkEntry(parts[0].Trim(), ts, isUserBookmark: true));
                }
            }
        }

        OnBookmarksChanged?.Invoke(new List<BookmarkEntry>(_userBookmarks));
    }

    /// <summary>Saves a user bookmark at the current playback time.</summary>
    /// <param name="label">Optional label; defaults to "Bookmark MM:SS".</param>
    public void AddBookmarkAtCurrentTime(string label = null)
    {
        if (_src.clip == null) return;

        float t = _src.time;
        if (string.IsNullOrWhiteSpace(label))
            label = $"Bookmark {TimeSpan.FromSeconds(t):mm\\:ss}";

        _userBookmarks.Add(new BookmarkEntry(label, t, isUserBookmark: true));
        _userBookmarks.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));
        SaveUserBookmarks();
        OnBookmarksChanged?.Invoke(new List<BookmarkEntry>(_userBookmarks));
    }

    /// <summary>Removes a user-created bookmark. Silently ignores authored chapters.</summary>
    public void RemoveBookmark(BookmarkEntry entry)
    {
        if (!entry.IsUserBookmark)
        {
            Debug.LogWarning("[AudiobookPlayer] Cannot remove authored chapters — only user bookmarks.");
            return;
        }
        _userBookmarks.Remove(entry);
        SaveUserBookmarks();
        OnBookmarksChanged?.Invoke(new List<BookmarkEntry>(_userBookmarks));
    }

    private void SaveUserBookmarks()
    {
        if (string.IsNullOrEmpty(_bookmarksPath)) return;
        var lines = new List<string> { "# User bookmarks — auto-generated" };
        foreach (var b in _userBookmarks)
            lines.Add($"{b.Label},{b.Timestamp.ToString(CultureInfo.InvariantCulture)}");
        File.WriteAllLines(_bookmarksPath, lines);
    }

    // ─── Playback Controls ────────────────────────────────────────────────────

    public void Play()
    {
        if (_src.clip == null) return;
        _src.Play();
        SetState(PlaybackState.Playing);

        if (_progressRoutine != null) StopCoroutine(_progressRoutine);
        _progressRoutine = StartCoroutine(ProgressReporter());
    }

    public void Pause()
    {
        if (CurrentState != PlaybackState.Playing) return;
        _src.Pause();
        SetState(PlaybackState.Paused);
    }

    public void TogglePlayPause()
    {
        if (CurrentState == PlaybackState.Playing) Pause();
        else Play();
    }

    public void Stop()
    {
        if (_progressRoutine != null) StopCoroutine(_progressRoutine);
        _src.Stop();
        _src.time = 0f;
        SetState(PlaybackState.Stopped);
        OnPlaybackProgressChanged?.Invoke(0f, TotalTime);
    }

    /// <summary>Seek to an absolute time in seconds.</summary>
    public void SeekTo(float seconds)
    {
        if (_src.clip == null) return;
        _src.time = Mathf.Clamp(seconds, 0f, _src.clip.length - 0.05f);
        OnPlaybackProgressChanged?.Invoke(_src.time, TotalTime);
    }

    /// <summary>Jump to the start of a chapter or bookmark entry.</summary>
    public void JumpToEntry(BookmarkEntry entry) => SeekTo(entry.Timestamp);

    /// <summary>Jump to the next chapter/bookmark after the current time.</summary>
    public void NextChapter()
    {
        float t    = CurrentTime;
        var   all  = GetAllEntriesSorted();
        var   next = all.Find(b => b.Timestamp > t + 0.5f);
        if (next != null) SeekTo(next.Timestamp);
    }

    /// <summary>Jump to the previous chapter/bookmark, with a 3-second grace window for replaying the current one.</summary>
    public void PreviousChapter()
    {
        float t    = CurrentTime;
        var   all  = GetAllEntriesSorted();
        var   prev = all.FindLast(b => b.Timestamp < t - 3f);
        SeekTo(prev?.Timestamp ?? 0f);
    }

    // ─── Queries ──────────────────────────────────────────────────────────────

    /// <summary>Returns chapters and user bookmarks merged and sorted by timestamp.</summary>
    public List<BookmarkEntry> GetAllEntriesSorted()
    {
        var all = new List<BookmarkEntry>(_chapters);
        all.AddRange(_userBookmarks);
        all.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));
        return all;
    }

    public List<BookmarkEntry> GetChapters()      => new(_chapters);
    public List<BookmarkEntry> GetUserBookmarks() => new(_userBookmarks);
    public List<string>        GetAvailableFiles() => new(_availableFiles);

    /// <summary>Returns the last chapter/bookmark whose timestamp is at or before currentTime.</summary>
    public BookmarkEntry GetCurrentChapter()
    {
        var   all     = GetAllEntriesSorted();
        float t       = CurrentTime;
        BookmarkEntry cur = null;
        foreach (var e in all)
            if (e.Timestamp <= t) cur = e;
        return cur;
    }

    // ─── Internal ─────────────────────────────────────────────────────────────

    private void SetState(PlaybackState state)
    {
        CurrentState = state;
        OnPlaybackStateChanged?.Invoke(state);
    }

    private IEnumerator ProgressReporter()
    {
        var wait = new WaitForSeconds(progressUpdateInterval);
        while (CurrentState == PlaybackState.Playing)
        {
            // Detect natural end-of-track
            if (!_src.isPlaying)
            {
                SetState(PlaybackState.Stopped);
                OnPlaybackProgressChanged?.Invoke(TotalTime, TotalTime);
                yield break;
            }
            OnPlaybackProgressChanged?.Invoke(_src.time, TotalTime);
            yield return wait;
        }
    }

    private static string StripExtension(string path)
        => Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
}
