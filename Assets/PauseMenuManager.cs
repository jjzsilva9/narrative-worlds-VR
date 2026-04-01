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
    [Tooltip("The pause menu panel (should have a CanvasGroup component)")]
    [SerializeField] private CanvasGroup pauseMenuPanel;

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
        // Hide pause menu at start
        SetCanvasGroupState(pauseMenuPanel, false);

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
        SetCanvasGroupState(pauseMenuPanel, isPaused);

        // TODO: Optionally pause/reduce audio when paused
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
        // Disable menu interaction so the user can't double-tap
        pauseMenuPanel.interactable = false;

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

    private void SetCanvasGroupState(CanvasGroup cg, bool active)
    {
        if (cg == null) return;

        cg.alpha = active ? 1f : 0f;
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }
}