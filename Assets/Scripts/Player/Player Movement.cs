using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float sprintSpeed = 12f;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float crouchWalkSpeed = 5f;

    [Space]
    [Tooltip("How quickly the player reaches max speed.")]
    [SerializeField] private float acceleration = 50f;
    [Tooltip("How quickly the player stops.")]
    [SerializeField] private float deceleration = 40f;

    /// <summary>
    /// Calculates a Vector3 to move the character controller with.
    /// Accounts for walk speed.
    /// </summary>
    /// <param name="deltaTime"> Global delta time. </param>
    /// <param name="moveInput"> A Vector2 with values from the input system. </param>
    /// <returns> A Vector3 with values used to move the character controller. </returns>
    public Vector3 UpdateMove(float deltaTime, Vector2 moveInput, ref Vector3 currentVelocity, bool isCrouching, bool isSprinting, bool isGrounded)
    {
        // Get the correct speed.
        float speed = calculateSpeed(isCrouching, isSprinting, isGrounded);

        // Get move inputs.
        float x = moveInput.x;
        float z = moveInput.y;

        // Create the movement vector.
        Vector3 move = transform.right * x + transform.forward * z;

        // Prevent diagonal movement from being faster.
        move = move.normalized;

        // Apply speed.
        Vector3 targetVelocity = move * speed;

        // Depending on input, apply acceleration or deceleration.
        float rate = move.sqrMagnitude > 0.1f ? acceleration : deceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, rate * deltaTime);

        // Return.
        return currentVelocity * deltaTime;
    }

    /// <summary>
    /// Returns the correct speed float based on the state of the player.
    /// </summary>
    /// <param name="isCrouching"> Is the player crouching? </param>
    /// <param name="isSprinting"> Is the player sprinting? </param>
    /// <param name="isGrounded"> Is the player grounded? </param>
    /// <returns></returns>
    private float calculateSpeed(bool isCrouching, bool isSprinting, bool isGrounded)
    {
        // If the player is crouched, use the crouch walk speed.
        // Ensure the player is grounded before changing speed, the player cannot sprint or crouch in the air.
        if (isCrouching & isGrounded)
        {
            return crouchWalkSpeed;
        }
        // If the player is sprinting, use the sprinting speed.
        else if (isSprinting && isGrounded)
        {
            return sprintSpeed;
        }
        // Otherwise return the walk speed.
        else
        {
            return walkSpeed;
        }
    }
}
