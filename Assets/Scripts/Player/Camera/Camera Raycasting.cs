using UnityEngine;

public class CameraRaycasting : MonoBehaviour
{
    [Tooltip("How far away the player will interact with an interactable object.")]
    [SerializeField] private float interactionDistance = 5f;

    // The stored current target's interactable script.
    private IInteractable _currentTarget;

    /// <summary>
    /// Checks if the player is looking at an interactable object, interact with it if called for.
    /// </summary>
    /// <param name="deltaTime"> Global delta time. </param>
    /// <param name="isInteracting"> If the interact button has been pressed. </param>
    public void UpdateRaycast(float deltaTime, bool isInteracting)
    {
        // Find an interactable.
        HandleRaycast();

        // If the interact button has been pressed.
        if(isInteracting)
        {
            // If there is an interactable object stored.
            if(_currentTarget != null)
            {
                // Interact with it.
                _currentTarget.OnInteract();
            }
        }
    }

    /// <summary>
    /// Shoots a raycast and finds an interactable script if there is one.
    /// </summary>
    private void HandleRaycast()
    {
        RaycastHit whatIHit;

        // If the raycast hit something.
        if(Physics.Raycast(transform.position, transform.forward, out whatIHit, interactionDistance))
        {
            IInteractable interactable = whatIHit.collider.GetComponent<IInteractable>();

            // If the object hit was interactable.
            if(interactable != null )
            {
                // If the object is the same as before, do nothing.
                if(interactable == _currentTarget )
                {
                    return;
                }
                // If the object is a new interactable:
                else if (_currentTarget != null)
                {
                    // Call the end look function on the old target.
                    _currentTarget.OnEndLook();
                    // Set the new object as the current interactable.
                    _currentTarget = interactable;
                    // Call the start look function on the new target.
                    _currentTarget.OnStartLook();
                }
                // Otherwise the object is new and we weren't prevously looking at anything.
                else
                {
                    // Set the object as the current target.
                    _currentTarget = interactable;
                    // Call it's start look function.
                    _currentTarget.OnStartLook();
                }
            }
            // If the object hit was not interactable.
            else
            {
                // If we were previously looking at something:
                if(_currentTarget != null)
                {
                    // Call it's end look function and set the current target to null.
                    _currentTarget.OnEndLook();
                    _currentTarget = null;
                }
            }
        }
        // If the raycast hit nothing.
        else
        {
            // If we were prevously looking at something:
            if(_currentTarget != null)
            {
                // Call it's end look function and set the current target to null.
                _currentTarget.OnEndLook();
                _currentTarget = null;
            }
        }
    }
}
