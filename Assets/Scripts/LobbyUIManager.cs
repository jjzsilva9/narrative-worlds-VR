using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the lobby UI flow: Title Screen -> Environment Selection -> Scene Transition.
/// Attach this to an empty GameObject in the lobby scene (e.g. "LobbyManager").
/// </summary>
public class LobbyUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The title/welcome screen panel with the 'Enter' button")]
    [SerializeField] private GameObject titlePanel;

    [Tooltip("The environment selection panel with scene buttons")]
    [SerializeField] private GameObject environmentPanel;

    [Header("Screen Fade")]
    [Tooltip("A full-screen CanvasGroup used for fading to/from black")]
    [SerializeField] private CanvasGroup screenFadeOverlay;

    [Tooltip("How long the screen fade to black takes")]
    [SerializeField] private float screenFadeDuration = 0.5f;

    [Header("Book")]
    [Tooltip("The book prefab to spawn when entering an environment")]
    [SerializeField] private GameObject bookPrefab;

    [Tooltip("Where the book should spawn (e.g. an empty GameObject positioned in front of the user)")]
    [SerializeField] private Transform bookSpawnPoint;

    [Header("Scene Names")]
    [Tooltip("Scene names for each environment - must match Build Settings")]
    [SerializeField] private string gollumsCaveSceneName = "GollumCave";
    [SerializeField] private string mordorSceneName = "Mordor";

    [Header("Audio")]
    [Tooltip("Ambient audio source that fades in when entering the Shire")]
    [SerializeField] private AudioSource ambientAudioSource;

    [Tooltip("Shire soundtrack audio source")]
    [SerializeField] private AudioSource musicAudioSource;

    [Tooltip("How long the audio takes to fade in")]
    [SerializeField] private float audioFadeDuration = 2.0f;

    [Header("Effects")]
    [Tooltip("Particle systems to enable when entering the Shire (e.g. fireflies, leaves)")]
    [SerializeField] private ParticleSystem[] shireParticleSystems;

    private void Start()
    {
        // Ensure correct initial state
        titlePanel.SetActive(true);
        environmentPanel.SetActive(false);

        // Make sure screen fade overlay is fully transparent at start
        if (screenFadeOverlay != null)
        {
            screenFadeOverlay.alpha = 0f;
            screenFadeOverlay.gameObject.SetActive(false);
        }

        // Disable particles at start
        if (shireParticleSystems != null)
        {
            foreach (var ps in shireParticleSystems)
            {
                if (ps != null) ps.Stop();
            }
        }

        // Mute ambient/music audio at start so it can fade in later
        if (ambientAudioSource != null)
            ambientAudioSource.volume = 0f;

        if (musicAudioSource != null)
            musicAudioSource.volume = 0f;
    }

    // ─────────────────────────────────────────────
    // BUTTON CALLBACKS - Hook these up in the Inspector
    // ─────────────────────────────────────────────

    /// <summary>
    /// Called when the user presses "Enter" on the title screen.
    /// Hides the title panel and shows the environment selection panel.
    /// </summary>
    public void OnEnterPressed()
    {
        TransitionTitleToEnvironmentSelect();
    }

    public void OnQuitPressed()
{
    Application.Quit();
}

    /// <summary>
    /// Called when the user selects "The Shire" button.
    /// Since we're already in the Shire, just dismiss the UI and reveal the environment.
    /// </summary>
    public void OnShireSelected()
    {
        StartCoroutine(EnterShireTransition());
    }

    /// <summary>
    /// Called when the user selects "Gollum's Cave" button.
    /// Fades to black and loads the Gollum's Cave scene.
    /// </summary>
    public void OnGollumsCaveSelected()
    {
        StartCoroutine(LoadSceneTransition(gollumsCaveSceneName));
    }

    /// <summary>
    /// Called when the user selects "Mordor" button.
    /// Fades to black and loads the Mordor scene.
    /// </summary>
    public void OnMordorSelected()
    {
        StartCoroutine(LoadSceneTransition(mordorSceneName));
    }

    // ─────────────────────────────────────────────
    // TRANSITION COROUTINES
    // ─────────────────────────────────────────────

    /// <summary>
    /// Hides title screen, shows environment selection.
    /// </summary>
    private void TransitionTitleToEnvironmentSelect()
    {
        titlePanel.SetActive(false);
        environmentPanel.SetActive(true);
    }

    /// <summary>
    /// The Shire transition: fade to black, dismiss UI, spawn book,
    /// fade back in with audio and particles.
    /// </summary>
    private IEnumerator EnterShireTransition()
    {
        // Fade screen to black
        yield return StartCoroutine(FadeScreen(0f, 1f, screenFadeDuration));

        // While screen is black: hide the UI
        environmentPanel.SetActive(false);

        // Spawn the book
        SpawnBook();

        // Enable particle systems
        EnableShireParticles();

        // Small pause while screen is black for dramatic effect
        yield return new WaitForSeconds(0.3f);

        // Fade screen back in
        yield return StartCoroutine(FadeScreen(1f, 0f, screenFadeDuration));

        // Fade in ambient audio and music
        StartCoroutine(FadeInAudio());
    }

    /// <summary>
    /// Generic scene load transition: fade to black, load scene.
    /// </summary>
    private IEnumerator LoadSceneTransition(string sceneName)
    {
        // Fade screen to black
        yield return StartCoroutine(FadeScreen(0f, 1f, screenFadeDuration));

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }

    // ─────────────────────────────────────────────
    // HELPER METHODS
    // ─────────────────────────────────────────────

    private void SpawnBook()
    {
        if (bookPrefab == null || bookSpawnPoint == null)
        {
            Debug.LogWarning("LobbyUIManager: Book prefab or spawn point not assigned.");
            return;
        }

        GameObject book = Instantiate(bookPrefab, bookSpawnPoint.position, bookSpawnPoint.rotation);

        if (book.TryGetComponent(out VRBookController bookController))
            bookController.SetPages(NarrativeChapters.Shire);
    }

    private void EnableShireParticles()
    {
        // TODO: Assign particle systems in Inspector once they're created
        if (shireParticleSystems != null)
        {
            foreach (var ps in shireParticleSystems)
            {
                if (ps != null) ps.Play();
            }
        }
    }

    private IEnumerator FadeInAudio()
    {
        // TODO: These audio sources should be assigned in Inspector
        // once Jaden's audio system is integrated

        float elapsed = 0f;

        float ambientTargetVolume = 0.6f; // TODO: Adjust to taste
        float musicTargetVolume = 0.4f;   // TODO: Adjust to taste

        while (elapsed < audioFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / audioFadeDuration;

            if (ambientAudioSource != null)
                ambientAudioSource.volume = Mathf.Lerp(0f, ambientTargetVolume, t);

            if (musicAudioSource != null)
                musicAudioSource.volume = Mathf.Lerp(0f, musicTargetVolume, t);

            yield return null;
        }
    }

    /// <summary>
    /// Fades the screen overlay (black screen) from one alpha to another.
    /// </summary>
    private IEnumerator FadeScreen(float from, float to, float duration)
    {
        if (screenFadeOverlay == null)
        {
            Debug.LogWarning("LobbyUIManager: Screen fade overlay not assigned.");
            yield break;
        }

        float elapsed = 0f;
        screenFadeOverlay.alpha = from;
        screenFadeOverlay.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            screenFadeOverlay.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        screenFadeOverlay.alpha = to;

        // If fully transparent, disable the overlay
        if (to <= 0f)
            screenFadeOverlay.gameObject.SetActive(false);
    }

}