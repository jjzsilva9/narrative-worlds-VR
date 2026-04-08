using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// In-scene pause menu that toggles when the user presses the menu button.
/// Provides "Return to Lobby" and a volume slider.
/// Attach this to a GameObject in each environment scene.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The pause menu panel")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Tooltip("The audiobook player panel (AudiobookPlayerRoot prefab). Leave unassigned to disable.")]
    [SerializeField] private GameObject audiobookPlayerPanel;

    [Tooltip("Horizontal distance (meters) between the main menu and the audiobook panel")]
    [SerializeField] private float audiobookPanelOffset = 0.6f;

    [Tooltip("Spawn the audiobook panel to the right of the main menu (unchecked = left)")]
    [SerializeField] private bool audiobookPanelOnRight = true;


    [Header("Positioning")]
    [Tooltip("The main camera (XR headset). If not assigned, will find Camera.main at start")]
    [SerializeField] private Camera playerCamera;

    [Tooltip("How far in front of the user the menu spawns (in meters)")]
    [SerializeField] private float spawnDistance = 1.5f;

    [Header("Input")]
    [Tooltip("Input action reference for the menu button (e.g. XRI LeftHand/Menu Button)")]
    [SerializeField] private InputActionReference menuButtonAction;

    [Header("Transition")]
    [SerializeField] private CanvasGroup screenFadeOverlay;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Scene Names")]
    [SerializeField] private string shireSceneName = "Shire";
    [SerializeField] private string gollumSceneName = "gollumscenetest";
    [SerializeField] private string mordorSceneName = "Mordor";

    [Header("Teleport")]
    [Tooltip("The XR Origin root transform to move before scene transition")]
    [SerializeField] private Transform xrOrigin;
    [Tooltip("The teleport anchor/platform in this scene to move the player to before fading out")]
    [SerializeField] private Transform teleportAnchor;

    private bool isPaused = false;
    private CanvasGroup _audiobookCanvasGroup;

    private void OnEnable()
    {
        if (menuButtonAction != null && menuButtonAction.action != null)
        {
            menuButtonAction.action.Enable();
            menuButtonAction.action.performed += OnMenuButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (menuButtonAction != null && menuButtonAction.action != null)
        {
            menuButtonAction.action.performed -= OnMenuButtonPressed;
        }
    }

    private void Start()
    {
        // Find camera if not assigned
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Hide panels at start
        pauseMenuPanel.SetActive(false);
        if (audiobookPlayerPanel != null)
        {
            audiobookPlayerPanel.SetActive(true); // keep active so audio runs
            _audiobookCanvasGroup = audiobookPlayerPanel.GetComponent<CanvasGroup>();
            if (_audiobookCanvasGroup == null)
                _audiobookCanvasGroup = audiobookPlayerPanel.AddComponent<CanvasGroup>();
            SetAudiobookPanelVisible(false);
        }

        if (screenFadeOverlay != null)
        {
            screenFadeOverlay.alpha = 0f;
            screenFadeOverlay.gameObject.SetActive(false);
        }

        StartCoroutine(TeleportNextFrame());
    }

    private IEnumerator TeleportNextFrame()
    {
        if (xrOrigin == null || teleportAnchor == null) yield break;

        // Keep retrying until the position sticks (XR simulator may override us)
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            yield return null;
            xrOrigin.position = teleportAnchor.position;
            xrOrigin.rotation = teleportAnchor.rotation;

            yield return null;
            if (Vector3.Distance(xrOrigin.position, teleportAnchor.position) < 0.05f)
                break;
        }
    }

    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PositionInFrontOfUser();
            pauseMenuPanel.SetActive(true);
            if (audiobookPlayerPanel != null)
                SetAudiobookPanelVisible(true);
        }
        else
        {
            pauseMenuPanel.SetActive(false);
            if (audiobookPlayerPanel != null)
                SetAudiobookPanelVisible(false);
        }

        // TODO: Optionally pause/reduce audio when paused
    }

    /// <summary>
    /// Positions the pause menu panel in front of the user's current view direction.
    /// </summary>
    private void PositionInFrontOfUser()
    {
        if (playerCamera == null) return;

        // Get the camera's forward direction but keep it level (ignore vertical tilt)
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // Place the panel in front of the user at eye height
        Vector3 spawnPosition = playerCamera.transform.position + forward * spawnDistance;
        pauseMenuPanel.transform.position = spawnPosition;

        // Rotate the panel to face the user
        pauseMenuPanel.transform.rotation = Quaternion.LookRotation(forward);

        // Position the audiobook panel to the left or right of the main menu
        if (audiobookPlayerPanel != null)
        {
            Vector3 right = playerCamera.transform.right;
            right.y = 0f;
            right.Normalize();

            float side = audiobookPanelOnRight ? 1f : -1f;
            Vector3 panelPosition = spawnPosition + right * (audiobookPanelOffset * side);
            audiobookPlayerPanel.transform.position = panelPosition;

            // Face the panel directly at the player
            Vector3 dirToPlayer = playerCamera.transform.position - panelPosition;
            dirToPlayer.y = 0f;
            audiobookPlayerPanel.transform.rotation = Quaternion.LookRotation(-dirToPlayer.normalized);
        }
    }

    public void LoadShire()      => StartCoroutine(LoadSceneTransition(shireSceneName));
    public void LoadGollumCave() => StartCoroutine(LoadSceneTransition(gollumSceneName));
    public void LoadMordor()     => StartCoroutine(LoadSceneTransition(mordorSceneName));
    public void OnQuitPressed()  => Application.Quit();

    /// <summary>
    /// Called by the volume slider's OnValueChanged().
    /// Expects a value from 0 to 1.
    /// </summary>
    public void OnVolumeChanged(float value)
    {
        // TODO: Hook this up to Jaden's audio system once it's implemented.
        // For now, we just set the global audio listener volume.
        AudioListener.volume = value;
    }

    private void SetAudiobookPanelVisible(bool visible)
    {
        if (_audiobookCanvasGroup == null) return;
        _audiobookCanvasGroup.alpha = visible ? 1f : 0f;
        _audiobookCanvasGroup.interactable = visible;
        _audiobookCanvasGroup.blocksRaycasts = visible;
    }

    private IEnumerator LoadSceneTransition(string sceneName)
    {
        pauseMenuPanel.SetActive(false);
        if (audiobookPlayerPanel != null)
            SetAudiobookPanelVisible(false);

        // Move player to teleport anchor before fading
        if (xrOrigin != null && teleportAnchor != null)
        {
            xrOrigin.position = teleportAnchor.position;
            xrOrigin.rotation = teleportAnchor.rotation;
        }

        if (screenFadeOverlay != null)
        {
            screenFadeOverlay.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                screenFadeOverlay.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            screenFadeOverlay.alpha = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }

}