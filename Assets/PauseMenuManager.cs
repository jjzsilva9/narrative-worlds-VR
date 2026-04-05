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

    [Header("Scene")]
    [SerializeField] private string lobbySceneName = "LobbyScene"; // TODO: Replace with actual lobby scene name

    private bool isPaused = false;

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

        // Hide pause menu at start
        pauseMenuPanel.SetActive(false);

        if (screenFadeOverlay != null)
        {
            screenFadeOverlay.alpha = 0f;
            screenFadeOverlay.gameObject.SetActive(false);
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
        }
        else
        {
            pauseMenuPanel.SetActive(false);
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
    }

    /// <summary>
    /// Called by the "Return to Lobby" button's OnClick().
    /// </summary>
    public void OnReturnToLobby()
    {
        StartCoroutine(ReturnToLobbyTransition());
    }

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

    private IEnumerator ReturnToLobbyTransition()
    {
        // Disable menu so the user can't double-tap
        pauseMenuPanel.SetActive(false);

        // Fade to black
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

        // Load lobby
        SceneManager.LoadScene(lobbySceneName);
    }

}