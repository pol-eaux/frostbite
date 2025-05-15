using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float cameraCrouchHeight = 0.75f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float cameraStandHeight = 1.5f;

    // The y value for the character controller center vector.
    private float _crouchCenter;
    private float _standCenter;

    // The y value for the character model scale.
    private float _modelCrouchScale = 0.5f;
    private float _modelStandScale = 1f;

    // Component references.
    private CharacterController _characterController;
    private Transform _modelTransform;
    private Transform _cameraTransform;

    // Keep track of stance.
    private bool _isCrouching;

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

        // Player is standing by default.
        _isCrouching = false;
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
        // If the player is in the air, do nothing.
        if (!isGrounded)
            return;

        // If the stance of the player hasn't changed, return.
        if (_isCrouching == crouchPressed)
            return;

        // Set stance.
        _isCrouching = crouchPressed;

        // Targets that will change depending on stance.
        float heightTarget;
        float centerTarget;
        float scaleTarget;
        float cameraHeightTarget;

        // If the player is holding crouch, use crouch variables.
        if(crouchPressed)
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

        // Set the player's height and center.
        _characterController.height = heightTarget;
        _characterController.center = new Vector3(0f, centerTarget, 0f);
        // Scale the model on the y-axis.
        _modelTransform.localScale = new Vector3(_modelTransform.localScale.x, scaleTarget, _modelTransform.localScale.z);
        // Set the height of the camera.
        _cameraTransform.localPosition = new Vector3(_cameraTransform.localPosition.x, cameraHeightTarget, _cameraTransform.localPosition.z);
    }
}
