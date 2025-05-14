using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float sprintSpeed = 12f;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float crouchWalkSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 40f;

    /// <summary>
    /// Calculates a Vector3 to move the character controller with.
    /// Accounts for walk speed.
    /// </summary>
    /// <param name="deltaTime"> Global delta time. </param>
    /// <param name="moveInput"> A Vector2 with values from the input system. </param>
    /// <returns> A Vector3 with values used to move the character controller. </returns>
    public Vector3 UpdateMove(float deltaTime, Vector2 moveInput, ref Vector3 currentVelocity, bool crouched, bool sprinting)
    {
        // Get the correct speed.
        float speed = calculateSpeed(crouched, sprinting);

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
    /// <param name="crouched"> Is the player crouching? </param>
    /// <param name="sprinting"> Is the player sprinting? </param>
    /// <returns></returns>
    private float calculateSpeed(bool crouched, bool sprinting)
    {
        // If the player is crouched, use the crouch walk speed.
        if (crouched)
        {
            return crouchWalkSpeed;
        }
        // If the player is sprinting, use the sprinting speed.
        else if (sprinting)
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
