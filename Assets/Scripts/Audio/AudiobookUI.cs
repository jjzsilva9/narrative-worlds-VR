using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the World Space Canvas UI for the Audiobook Player.
/// Attach to the same root GameObject as AudiobookPlayer.
///
/// UI HIERARCHY EXPECTED (wire every [SerializeField] in the Inspector):
///
///   Canvas (World Space)
///   └── AudiobookPlayerRoot          ← this script lives here
///       ├── Header
///       │   └── TitleLabel           ← TMP_Text
///       ├── FileListPanel
///       │   └── ScrollView
///       │       └── Content          ← fileListContainer (Vertical Layout Group)
///       ├── NowPlayingPanel
///       │   ├── NowPlayingLabel      ← TMP_Text  (track name)
///       │   ├── ChapterLabel         ← TMP_Text  (current chapter)
///       │   ├── ScrubSlider          ← Slider
///       │   ├── TimeLabel            ← TMP_Text  "02:05 / 18:30"
///       │   └── Controls
///       │       ├── PrevButton       ← Button
///       │       ├── PlayPauseButton  ← Button  (+PlayPauseIcon TMP_Text child)
///       │       ├── NextButton       ← Button
///       │       ├── StopButton       ← Button
///       │       └── AddBookmarkButton← Button
///       └── BookmarkPanel
///           ├── BookmarkListLabel    ← TMP_Text  (section heading)
///           └── ScrollView
///               └── Content         ← bookmarkListContainer (Vertical Layout Group)
///
/// PREFAB SETUP:
///   See README.md or the implementation plan for full step-by-step instructions.
/// </summary>
[RequireComponent(typeof(AudiobookPlayer))]
public class AudiobookUI : MonoBehaviour
{
    // ─── Player Reference ─────────────────────────────────────────────────────

    private AudiobookPlayer _player;

    // ─── Inspector Wiring ─────────────────────────────────────────────────────

    [Header("Now Playing")]
    [SerializeField] private TMP_Text nowPlayingLabel;
    [SerializeField] private TMP_Text chapterLabel;
    [SerializeField] private TMP_Text timeLabel;

    [Header("Scrub Slider")]
    [SerializeField] private Slider scrubSlider;

    [Header("Playback Buttons")]
    [SerializeField] private Button   prevButton;
    [SerializeField] private Button   playPauseButton;
    [SerializeField] private TMP_Text playPauseIcon;   // child TMP label on the button — set to "▶" or "⏸"
    [SerializeField] private Button   nextButton;
    [SerializeField] private Button   stopButton;
    [SerializeField] private Button   addBookmarkButton;

    [Header("File List")]
    [SerializeField] private Transform  fileListContainer;   // Vertical Layout Group parent
    [SerializeField] private GameObject fileButtonPrefab;    // prefab: Button + TMP_Text child

    [Header("Bookmark / Chapter List")]
    [SerializeField] private Transform  bookmarkListContainer; // Vertical Layout Group parent
    [SerializeField] private GameObject bookmarkRowPrefab;     // prefab: two TMP_Text + optional delete Button

    [Header("Loading Overlay")]
    [Tooltip("Optional panel to show while a file is loading.")]
    [SerializeField] private GameObject loadingOverlay;

    [Header("Icons")]
    [SerializeField] private string iconPlay    = ">";
    [SerializeField] private string iconPause   = "||";  // ASCII pause
    [SerializeField] private string iconPlaying = "*";  // indicator for active file in list
    [SerializeField] private string iconChapter = "-";  // authored chapter prefix
    [SerializeField] private string iconBookmark = "[B]"; // user bookmark prefix

    // ─── Private State ────────────────────────────────────────────────────────

    private bool  _scrubbing;   // true while the user is dragging the slider
    private float _totalTime;

    // Keep track of instantiated rows so we can rebuild cleanly
    private readonly List<GameObject> _fileRows     = new();
    private readonly List<GameObject> _bookmarkRows = new();

    // Map file path → its row GameObject for highlighting active file
    private readonly Dictionary<string, GameObject> _fileRowMap = new();

    private string _currentFilePath;

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    private void Awake()
    {
        _player = GetComponent<AudiobookPlayer>();
    }

    private void Start()
    {
        // Subscribe to player events
        _player.OnFilesDiscovered       += HandleFilesDiscovered;
        _player.OnFileLoaded            += HandleFileLoaded;
        _player.OnFileLoadFailed        += HandleFileLoadFailed;
        _player.OnChaptersLoaded        += HandleChaptersLoaded;
        _player.OnBookmarksChanged      += HandleBookmarksChanged;
        _player.OnPlaybackStateChanged  += HandlePlaybackStateChanged;
        _player.OnPlaybackProgressChanged += HandleProgressChanged;

        // Wire buttons
        prevButton?.onClick.AddListener(_player.PreviousChapter);
        playPauseButton?.onClick.AddListener(_player.TogglePlayPause);
        nextButton?.onClick.AddListener(_player.NextChapter);
        stopButton?.onClick.AddListener(_player.Stop);
        addBookmarkButton?.onClick.AddListener(OnAddBookmarkPressed);

        // Wire scrub slider
        if (scrubSlider != null)
        {
            scrubSlider.onValueChanged.AddListener(OnScrubValueChanged);
            // Detect drag start/end to avoid fighting the AudioSource.time during drag
            var trigger = scrubSlider.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            AddEventTrigger(trigger, UnityEngine.EventSystems.EventTriggerType.PointerDown,
                _ => _scrubbing = true);
            AddEventTrigger(trigger, UnityEngine.EventSystems.EventTriggerType.PointerUp,
                _ => { _player.SeekTo(scrubSlider.value * _totalTime); _scrubbing = false; });
        }

        // Initial UI state
        SetNowPlaying(null);
        SetPlayPauseIcon(false);
        ShowLoading(false);
    }

    private void OnDestroy()
    {
        if (_player == null) return;
        _player.OnFilesDiscovered         -= HandleFilesDiscovered;
        _player.OnFileLoaded              -= HandleFileLoaded;
        _player.OnFileLoadFailed          -= HandleFileLoadFailed;
        _player.OnChaptersLoaded          -= HandleChaptersLoaded;
        _player.OnBookmarksChanged        -= HandleBookmarksChanged;
        _player.OnPlaybackStateChanged    -= HandlePlaybackStateChanged;
        _player.OnPlaybackProgressChanged -= HandleProgressChanged;
    }

    // ─── Player Event Handlers ────────────────────────────────────────────────

    private void HandleFilesDiscovered(List<string> files)
    {
        RebuildFileList(files);
    }

    private void HandleFileLoaded(string displayName)
    {
        ShowLoading(false);
        SetNowPlaying(displayName);
        HighlightActiveFile(_currentFilePath);
        // Reset scrub slider
        if (scrubSlider != null) { scrubSlider.value = 0f; }
        if (timeLabel != null)   timeLabel.text = "0:00 / --:--";
    }

    private void HandleFileLoadFailed()
    {
        ShowLoading(false);
        if (nowPlayingLabel != null) nowPlayingLabel.text = "[!] Failed to load file";
    }

    private void HandleChaptersLoaded(List<AudiobookPlayer.BookmarkEntry> chapters)
    {
        // Bookmark list is rebuilt when bookmarks also arrive (HandleBookmarksChanged).
        // If there are no chapters, we still do a rebuild with whatever bookmarks exist.
        RebuildBookmarkList(_player.GetAllEntriesSorted());
    }

    private void HandleBookmarksChanged(List<AudiobookPlayer.BookmarkEntry> _)
    {
        RebuildBookmarkList(_player.GetAllEntriesSorted());
    }

    private void HandlePlaybackStateChanged(AudiobookPlayer.PlaybackState state)
    {
        bool playing = state == AudiobookPlayer.PlaybackState.Playing;
        SetPlayPauseIcon(playing);
        ShowLoading(state == AudiobookPlayer.PlaybackState.Loading);
    }

    private void HandleProgressChanged(float current, float total)
    {
        _totalTime = total;

        if (timeLabel != null)
            timeLabel.text = $"{FormatTime(current)} / {FormatTime(total)}";

        if (scrubSlider != null && !_scrubbing && total > 0f)
            scrubSlider.SetValueWithoutNotify(current / total);

        // Update current chapter label
        var cur = _player.GetCurrentChapter();
        if (chapterLabel != null)
            chapterLabel.text = cur != null ? $"{(cur.IsUserBookmark ? iconBookmark : iconChapter)} {cur.Label}" : "";
    }

    // ─── File List ────────────────────────────────────────────────────────────

    private void RebuildFileList(List<string> files)
    {
        ClearRows(_fileRows);
        _fileRowMap.Clear();

        if (fileListContainer == null || fileButtonPrefab == null) return;

        foreach (string path in files)
        {
            string displayName = Path.GetFileNameWithoutExtension(path);
            string capturedPath = path; // closure capture

            GameObject row = Instantiate(fileButtonPrefab, fileListContainer);
            _fileRows.Add(row);
            _fileRowMap[path] = row;

            // Set label
            var label = row.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = displayName;

            // Load on click
            var btn = row.GetComponent<Button>();
            btn?.onClick.AddListener(() => OnFileSelected(capturedPath));
        }
    }

    private void OnFileSelected(string fullPath)
    {
        _currentFilePath = fullPath;
        ShowLoading(true);

        // Clear bookmark list while new file loads
        ClearRows(_bookmarkRows);
        if (chapterLabel != null) chapterLabel.text = "";

        _player.LoadFile(fullPath);
        // Auto-play after load
        StartCoroutine(WaitThenPlay());
    }

    private System.Collections.IEnumerator WaitThenPlay()
    {
        // Wait until the player leaves the Loading state
        while (_player.CurrentState == AudiobookPlayer.PlaybackState.Loading)
            yield return null;

        if (_player.CurrentState == AudiobookPlayer.PlaybackState.Stopped)
            _player.Play();
    }

    private void HighlightActiveFile(string activePath)
    {
        foreach (var kvp in _fileRowMap)
        {
            var label = kvp.Value.GetComponentInChildren<TMP_Text>();
            if (label == null) continue;
            string baseName = Path.GetFileNameWithoutExtension(kvp.Key);
            bool   active   = kvp.Key == activePath;
            label.text      = active ? $"{iconPlaying} {baseName}" : baseName;
        }
    }

    // ─── Bookmark / Chapter List ──────────────────────────────────────────────

    private void RebuildBookmarkList(List<AudiobookPlayer.BookmarkEntry> entries)
    {
        ClearRows(_bookmarkRows);

        if (bookmarkListContainer == null || bookmarkRowPrefab == null) return;

        foreach (var entry in entries)
        {
            var   captured = entry;
            string prefix  = entry.IsUserBookmark ? iconBookmark : iconChapter;

            GameObject row = Instantiate(bookmarkRowPrefab, bookmarkListContainer);
            _bookmarkRows.Add(row);

            // Find labels by name or index
            var texts = row.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 1) texts[0].text = $"{prefix} {entry.Label}";
            if (texts.Length >= 2) texts[1].text = FormatTime(entry.Timestamp);

            // Jump to timestamp on click  (click the row background)
            var rowBtn = row.GetComponent<Button>();
            rowBtn?.onClick.AddListener(() => _player.JumpToEntry(captured));

            // Delete button — find any child Button that isn't the row root itself,
            // so this works regardless of what name the prefab uses.
            Button deleteBtn = null;
            foreach (var b in row.GetComponentsInChildren<Button>(includeInactive: true))
            {
                if (b.gameObject != row)   // skip the row's own Button
                {
                    deleteBtn = b;
                    break;
                }
            }

            if (deleteBtn != null)
            {
                // Use CanvasGroup so the button is invisible but still occupies layout space
                var cg = deleteBtn.GetComponent<CanvasGroup>();
                if (cg == null) cg = deleteBtn.gameObject.AddComponent<CanvasGroup>();
                cg.alpha          = entry.IsUserBookmark ? 1f : 0f;
                cg.interactable   = entry.IsUserBookmark;
                cg.blocksRaycasts = entry.IsUserBookmark;

                deleteBtn.onClick.AddListener(() =>
                {
                    _player.RemoveBookmark(captured);
                    // List rebuilds automatically via OnBookmarksChanged
                });
            }
            else
            {
                Debug.LogWarning("[AudiobookUI] bookmarkRowPrefab has no child Button for delete. " +
                                 "Add a child Button to the prefab.");
            }
        }
    }

    // ─── Bookmark Input ───────────────────────────────────────────────────────

    private void OnAddBookmarkPressed()
    {
        // Simple default label — you could replace this with a VR keyboard / InputField
        _player.AddBookmarkAtCurrentTime();
    }

    // ─── Scrub ────────────────────────────────────────────────────────────────

    private void OnScrubValueChanged(float normalised)
    {
        // Only update the time display while dragging; actual seek happens on PointerUp
        if (_scrubbing && timeLabel != null)
            timeLabel.text = $"{FormatTime(normalised * _totalTime)} / {FormatTime(_totalTime)}";
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private void SetNowPlaying(string name)
    {
        if (nowPlayingLabel != null)
            nowPlayingLabel.text = name != null ? name : "No file loaded";
    }

    private void SetPlayPauseIcon(bool playing)
    {
        if (playPauseIcon != null)
            playPauseIcon.text = playing ? iconPause : iconPlay;
    }

    private void ShowLoading(bool show)
    {
        if (loadingOverlay != null)
            loadingOverlay.SetActive(show);
    }

    private static string FormatTime(float seconds)
    {
        var ts = TimeSpan.FromSeconds(Mathf.Max(0f, seconds));
        return ts.TotalHours >= 1
            ? $"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}"
            : $"{ts.Minutes}:{ts.Seconds:D2}";
    }

    private static void ClearRows(List<GameObject> rows)
    {
        foreach (var r in rows)
            if (r != null) Destroy(r);
        rows.Clear();
    }

    private static void AddEventTrigger(
        UnityEngine.EventSystems.EventTrigger trigger,
        UnityEngine.EventSystems.EventTriggerType type,
        Action<UnityEngine.EventSystems.BaseEventData> callback)
    {
        var entry = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(data => callback(data));
        trigger.triggers.Add(entry);
    }
}
