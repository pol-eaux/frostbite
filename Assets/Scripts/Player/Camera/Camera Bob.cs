using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [SerializeField] private bool enableHeadbob = true;

    [Space]
    [Tooltip("How frequent the camera will bob while walking.")]
    [SerializeField] private float walkBobFrequency = 10f;
    [Tooltip("How frequent the camera will bob while crouch walking.")]
    [SerializeField] private float crouchWalkBobFrequency = 5f;
    [Tooltip("How frequent the camera will bob while sprinting.")]
    [SerializeField] private float sprintBobFrequency = 12f;
    [Tooltip("How strong each bob is.")]
    [SerializeField] private float amplitude = 0.05f;
    [Tooltip("How fast the camera position will be reset when standing still.")]
    [SerializeField] private float resetSmoothing = 5f;

    private CharacterController _characterController;
    private float _bobTimer;

    // The starting position of the CameraBob object.
    private Vector3 _startPosition;
    // Clamp for the start position.
    private const float returnThreshold = 0.001f; 

    /// <summary>
    /// Initializes the camera bob script by getting and assigning needed variables.
    /// </summary>
    /// <param name="characterController"> A reference to the player character controller. </param>
    public void Initialize(CharacterController characterController)
    {
        _characterController = characterController;
        _startPosition = transform.localPosition;
        _bobTimer = 0f;
    }

    /// <summary>
    /// Updates the local position of the Camera Bob object based on if the player is moving.
    /// If the player is not moving, the local position will be reset to the start using LERP.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdateBob(float deltaTime, bool isGrounded, bool isCrouching, bool isSprinting)
    {
        // If headbob is disabled, do nothing.
        if (!enableHeadbob)
        {
            // If for some reason the camera is not at the start position, reset it.
            if(transform.localPosition != _startPosition)
            {
                transform.localPosition = _startPosition;
            }
            return;
        }

        // Determine frequency based on state.
        float frequency = walkBobFrequency;
        if (isCrouching && isGrounded)
        {
            frequency = crouchWalkBobFrequency;
        }
        else if (isSprinting)
        {
            frequency = sprintBobFrequency;
        }

        // If the player is moving, apply bob.
        if (IsPlayerMoving() && isGrounded)
        {
            _bobTimer += deltaTime * frequency;

            float horizontalBob = Mathf.Cos(_bobTimer) * amplitude;
            float verticalBob = Mathf.Sin(_bobTimer * 2f) * amplitude;

            // Apply transformation.
            transform.localPosition = _startPosition + new Vector3(horizontalBob, verticalBob, 0f);
        }
        // Else the player is stationary.
        else
        {
            // Start lerping towards the start position.
            transform.localPosition = Vector3.Lerp(transform.localPosition, _startPosition, deltaTime * resetSmoothing);

            // Clamp so that the original camera position is always reached.
            if ((transform.localPosition - _startPosition).sqrMagnitude < returnThreshold * returnThreshold) transform.localPosition = _startPosition;

            // Reset timer.
            _bobTimer = 0f;
        }
    }

    /// <summary>
    /// Gets the velocity of the character controller and compares it agains a given threshold to determine if the player is moving.
    /// </summary>
    /// <returns> Whether the player is moving or not. </returns>
    private bool IsPlayerMoving()
    {
        Vector3 horizontalVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.y);
        return horizontalVelocity.magnitude > 0.1f;
    }
}
