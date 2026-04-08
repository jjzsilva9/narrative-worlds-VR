using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Added to the spawned VRBook at runtime by SceneBookSpawner.
/// Fires SceneAudioSetup.OnBookPickedUp() the first time the book is grabbed.
/// </summary>
public class BookGrabHandler : MonoBehaviour
{
    private SceneAudioSetup _audioSetup;

    public void Init(SceneAudioSetup audioSetup)
    {
        _audioSetup = audioSetup;

        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
            grab.selectEntered.AddListener(OnGrabbed);
        else
            Debug.LogWarning("[BookGrabHandler] No XRGrabInteractable found on book.");
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _audioSetup?.OnBookPickedUp();
    }

    private void OnDestroy()
    {
        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrabbed);
    }
}
