using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float cameraCrouchHeight = 0.75f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float cameraStandHeight = 1.5f;

    [Space]
    [SerializeField] private float crouchTransitionSpeed = 5f;

    [Space]
    [SerializeField] private float headCheckTolerance = 0.51f;

    // The y value for the character controller center vector.
    private float _crouchCenter;
    private float _standCenter;

    // The y value for the character model scale.
    private float _modelCrouchScale = 0.5f;
    private float _modelStandScale = 1f;

    // For smooth transition.
    private float _currentHeight;
    private float _currentCenter;
    private float _currentScale;
    private float _currentCameraHeight;

    // Component references.
    private CharacterController _characterController;
    private Transform _modelTransform;
    private Transform _cameraTransform;

    // Collision.
    private int _layerMask;

    /// <summary>
    /// Provides the player crouch component with everything it needs to work.
    /// The character controller is needed to modify height and center values.
    /// </summary>
    /// <param name="characterController"> The player's main character controller. </param>
    public void Initialize(CharacterController characterController, Transform modelTransform, Transform cameraTransform)
    {
        // Assign necessary references.
        _characterController = characterController;
        _modelTransform = modelTransform;
        _cameraTransform = cameraTransform;
        // The center is always half of the height.
        _crouchCenter = crouchHeight / 2;
        _standCenter = standHeight / 2;

        // Set default values for smooth transition variables.
        _currentHeight = _characterController.height;
        _currentCenter = _characterController.center.y;
        _currentScale = _modelTransform.localScale.y;
        _currentCameraHeight = _cameraTransform.localPosition.y;

        // Prevent the head checker from interacting with the player.
        _layerMask = ~LayerMask.GetMask("Player");
    }

    /// <summary>
    /// Crouch if the crouch input is held, otherwise stand.
    /// Crouching only works if the palyer is grounded.
    /// </summary>
    /// <param name="deltaTime"> Global delta time. </param>
    /// <param name="crouchPressed"> Crouch input. </param>
    /// <param name="isGrounded"> Grounded bool from the character controller. </param>
    public void UpdateStance(float deltaTime, bool crouchPressed, bool isGrounded)
    {
        // Calculate where the ray should cast from.
        Vector3 castOrigin = transform.position + new Vector3(0, standHeight - headCheckTolerance, 0);
        // Check if the player is blocked from standing.
        bool blocked = Physics.CheckSphere(castOrigin, headCheckTolerance, _layerMask, QueryTriggerInteraction.Ignore);

        // Early exit if already in the target stance.
        if (!crouchPressed && Mathf.Approximately(_currentHeight, standHeight))
            return;
        if (crouchPressed && Mathf.Approximately(_currentHeight, crouchHeight))
            return;

        // Targets that will change depending on stance.
        float heightTarget;
        float centerTarget;
        float scaleTarget;
        float cameraHeightTarget;

        // For smooth transition.
        float crouchDelta = deltaTime * crouchTransitionSpeed;

        // If the player is holding crouch and is grounded, use crouch variables.
        // If the player is blocked from above, the player should stay crouched.
        if((crouchPressed && isGrounded) || blocked)
        {
            heightTarget = crouchHeight;
            centerTarget = _crouchCenter;
            scaleTarget = _modelCrouchScale;
            cameraHeightTarget = cameraCrouchHeight;
        }
        // Otherwise the player should stand, use stand variables.
        else
        {
            heightTarget = standHeight;
            centerTarget = _standCenter;
            scaleTarget = _modelStandScale;
            cameraHeightTarget = cameraStandHeight;
        }

        // Smooth Transition.
        _currentHeight = Mathf.Lerp(_currentHeight, heightTarget, crouchDelta);
        _currentCenter = Mathf.Lerp(_currentCenter, centerTarget, crouchDelta);
        _currentScale = Mathf.Lerp(_currentScale, scaleTarget, crouchDelta);
        _currentCameraHeight = Mathf.Lerp(_currentCameraHeight, cameraHeightTarget, crouchDelta);

        // Clamp to ensure exact match when very close.
        if (Mathf.Abs(_currentHeight - heightTarget) < 0.001f) _currentHeight = heightTarget;
        if (Mathf.Abs(_currentCenter - centerTarget) < 0.001f) _currentCenter = centerTarget;
        if (Mathf.Abs(_currentScale - scaleTarget) < 0.001f) _currentScale = scaleTarget;
        if (Mathf.Abs(_currentCameraHeight - cameraHeightTarget) < 0.001f) _currentCameraHeight = cameraHeightTarget;

        // Set the player's height and center.
        _characterController.height = _currentHeight;
        _characterController.center = new Vector3(0f, _currentCenter, 0f);
        // Scale the model on the y-axis.
        _modelTransform.localScale = new Vector3(_modelTransform.localScale.x, _currentScale, _modelTransform.localScale.z);
        // Set the height of the camera.
        _cameraTransform.localPosition = new Vector3(_cameraTransform.localPosition.x, _currentCameraHeight, _cameraTransform.localPosition.z);
    }
}
