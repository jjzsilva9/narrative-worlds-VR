using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns randomised 3D spatialized sounds at random positions around a centre point
/// for a configurable duration when the scene starts.
///
/// PREFAB SETUP:
///   1. Create an empty GameObject, name it "AmbientSoundSystem".
///   2. Add this component.
///   3. Assign AudioClips in the Inspector.
///   4. (Optional) Assign a SpawnCenter Transform — if left null, sounds spawn around this object.
///   5. Save as a Prefab and drop into any scene.
///
/// The orange wire-sphere gizmo shows the spawn radius in Scene view.
/// </summary>
public class AmbientSoundSpawner : MonoBehaviour
{
    // ─── Audio Clips ──────────────────────────────────────────────────────────

    [Header("Audio Clips")]
    [Tooltip("Pool of clips to randomly pick from.")]
    [SerializeField] private AudioClip[] audioClips;

    // ─── Timing ───────────────────────────────────────────────────────────────

    [Header("Timing")]
    [Tooltip("How many seconds after scene start to keep spawning sounds. Set to 0 for infinite.")]
    [SerializeField] private float activeDuration = 60f;

    [Tooltip("Minimum random delay between spawns (seconds).")]
    [SerializeField] private float minInterval = 2f;

    [Tooltip("Maximum random delay between spawns (seconds).")]
    [SerializeField] private float maxInterval = 8f;

    // ─── Spatial Settings ─────────────────────────────────────────────────────

    [Header("Spatial Settings")]
    [Tooltip("Maximum distance from SpawnCenter to place the audio source.")]
    [SerializeField] private float spawnRadius = 10f;

    [Tooltip("Optional centre point for spawning. Defaults to this GameObject if null.")]
    [SerializeField] private Transform spawnCenter;

    [Tooltip("0 = fully 2D, 1 = full 3D spatialized.")]
    [Range(0f, 1f)]
    [SerializeField] private float spatialBlend = 1f;

    [Tooltip("Keeps sounds above this Y offset from the spawn centre (avoid underground sounds).")]
    [SerializeField] private float minHeightOffset = -1f;

    // ─── Randomisation ────────────────────────────────────────────────────────

    [Header("Randomisation")]
    [Range(0f, 1f)]
    [SerializeField] private float minVolume = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float maxVolume = 1f;

    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    // ─── 3D Rolloff ───────────────────────────────────────────────────────────

    [Header("3D Rolloff")]
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    // ─────────────────────────────────────────────────────────────────────────

    private bool _isRunning;

    private void Start()
    {
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("[AmbientSoundSpawner] No audio clips assigned — assign clips in the Inspector.");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    // ─── Spawner Coroutine ────────────────────────────────────────────────────

    private IEnumerator SpawnRoutine()
    {
        _isRunning = true;
        float elapsed = 0f;
        bool infinite = activeDuration <= 0f;

        while (infinite || elapsed < activeDuration)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
            elapsed += wait;

            if (!infinite && elapsed >= activeDuration)
                break;

            SpawnSound();
        }

        _isRunning = false;
    }

    // ─── Sound Spawning ───────────────────────────────────────────────────────

    private void SpawnSound()
    {
        if (audioClips.Length == 0) return;

        AudioClip clip = audioClips[Random.Range(0, audioClips.Length)];

        // Random position within a sphere, clamped to minimum height
        Vector3 centre = spawnCenter != null ? spawnCenter.position : transform.position;
        Vector3 offset  = Random.insideUnitSphere * spawnRadius;
        offset.y        = Mathf.Max(offset.y, minHeightOffset);
        Vector3 spawnPos = centre + offset;

        // Temporary host object — self-destructs after the clip finishes
        GameObject soundObj = new GameObject($"[Ambient] {clip.name}");
        soundObj.transform.position = spawnPos;

        AudioSource src = soundObj.AddComponent<AudioSource>();
        src.clip          = clip;
        src.spatialBlend  = spatialBlend;
        src.volume        = Random.Range(minVolume, maxVolume);
        src.pitch         = Random.Range(minPitch, maxPitch);
        src.minDistance   = minDistance;
        src.maxDistance   = maxDistance;
        src.rolloffMode   = rolloffMode;
        src.playOnAwake   = false;
        src.loop          = false;
        src.Play();

        Destroy(soundObj, clip.length + 0.2f);
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    /// <summary>Force-stop all future spawning (does not cut currently playing sounds).</summary>
    public void StopSpawning()
    {
        StopAllCoroutines();
        _isRunning = false;
    }

    /// <summary>Restart the spawner (resets the elapsed timer).</summary>
    public void RestartSpawning()
    {
        StopSpawning();
        Start();
    }

    // ─── Editor Gizmos ───────────────────────────────────────────────────────

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 centre = spawnCenter != null ? spawnCenter.position : transform.position;

        UnityEditor.Handles.color = new Color(1f, 0.6f, 0.1f, 0.15f);
        UnityEditor.Handles.DrawSolidDisc(centre, Vector3.up, spawnRadius);

        Gizmos.color = new Color(1f, 0.6f, 0.1f, 0.9f);
        Gizmos.DrawWireSphere(centre, spawnRadius);
    }
#endif
}
