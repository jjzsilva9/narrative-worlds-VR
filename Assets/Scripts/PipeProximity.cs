using UnityEngine;

/// <summary>
/// Plays the animation on a child Animator when the pipe is close to the player's head.
/// Attach to the pipe prefab root. Assign the child Animator in the Inspector.
/// </summary>
public class PipeProximity : MonoBehaviour
{
    [Tooltip("The Animator on the pipe's child that holds the smoking/puffing animation")]
    [SerializeField] private Animator pipeAnimator;

    [Tooltip("The camera representing the player's head (XR headset). If unassigned, uses Camera.main")]
    [SerializeField] private Camera playerCamera;

    [Tooltip("The point on the pipe to measure distance from (e.g. the mouthpiece attach point). If unassigned, uses this object's position")]
    [SerializeField] private Transform attachPoint;

    [Tooltip("Distance in meters at which the animation begins playing")]
    [SerializeField] private float triggerDistance = 0.3f;

    [Tooltip("Name of the animation state to play")]
    [SerializeField] private string animationStateName = "Smoke";

    private bool isPlaying = false;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Disable the animator so it doesn't play until the pipe is near the player
        if (pipeAnimator != null)
            pipeAnimator.enabled = false;
    }

    private void Update()
    {
        if (playerCamera == null || pipeAnimator == null) return;

        Vector3 checkPosition = attachPoint != null ? attachPoint.position : transform.position;
        float distance = Vector3.Distance(checkPosition, playerCamera.transform.position);
        bool inRange = distance <= triggerDistance;

        if (inRange && !isPlaying)
        {
            pipeAnimator.enabled = true;
            pipeAnimator.Play(animationStateName);
            isPlaying = true;
        }
        else if (!inRange && isPlaying)
        {
            pipeAnimator.enabled = false;
            isPlaying = false;
        }
    }
}
