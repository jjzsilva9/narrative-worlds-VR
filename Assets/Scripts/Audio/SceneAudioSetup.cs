using System.Collections;
using UnityEngine;

/// <summary>
/// Place in each environment scene. Plays scene music on load via musicPlayer,
/// then when the book is picked up dims the music and starts the audiobook
/// on audiobookPlayer simultaneously.
/// Assign two separate AudiobookPlayer GameObjects in the Inspector.
/// </summary>
public class SceneAudioSetup : MonoBehaviour
{
    [Header("Players")]
    [Tooltip("AudiobookPlayer used for background scene music")]
    [SerializeField] private AudiobookPlayer musicPlayer;

    [Tooltip("AudiobookPlayer used for the audiobook narration")]
    [SerializeField] private AudiobookPlayer audiobookPlayer;

    [Header("Audio Clips")]
    [Tooltip("Music clip to play when the scene loads.")]
    [SerializeField] private AudioClip sceneMusicClip;

    [Tooltip("Chapters file for the music clip (optional).")]
    [SerializeField] private TextAsset sceneMusicChapters;

    [Tooltip("Audiobook clip to play when the book is picked up.")]
    [SerializeField] private AudioClip sceneAudiobookClip;

    [Tooltip("Chapters file for the audiobook clip (optional).")]
    [SerializeField] private TextAsset sceneAudiobookChapters;

    [Header("Music Ducking")]
    [Tooltip("Volume to fade music down to when the audiobook starts (0-1)")]
    [SerializeField] private float duckedMusicVolume = 0.2f;

    [Tooltip("How long the music fade takes in seconds")]
    [SerializeField] private float duckFadeDuration = 2f;

    private bool _audiobookStarted = false;

    private void Start()
    {
        if (sceneMusicClip != null)
            musicPlayer?.LoadClip(sceneMusicClip, sceneMusicChapters);
    }

    /// <summary>Called by BookGrabHandler when the book is first grabbed.</summary>
    public void OnBookPickedUp()
    {
        if (_audiobookStarted) return;
        _audiobookStarted = true;

        if (musicPlayer != null)
            StartCoroutine(DuckMusic());

        if (sceneAudiobookClip != null)
            audiobookPlayer?.LoadClip(sceneAudiobookClip, sceneAudiobookChapters);
    }

    private IEnumerator DuckMusic()
    {
        var src = musicPlayer.GetComponent<AudioSource>();
        if (src == null) yield break;

        float startVolume = src.volume;
        float elapsed = 0f;

        while (elapsed < duckFadeDuration)
        {
            elapsed += Time.deltaTime;
            src.volume = Mathf.Lerp(startVolume, duckedMusicVolume, elapsed / duckFadeDuration);
            yield return null;
        }

        src.volume = duckedMusicVolume;
    }
}
