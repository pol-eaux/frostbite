using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 15f;

    private Vector3 _lastPosition = Vector3.zero;

    /// <summary>
    /// Calculates a Vector3 to move the character controller with.
    /// Accounts for walk speed.
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <param name="moveInput"> A Vector2 with values from the input system. </param>
    /// <returns> A Vector3 with values used to move the character controller. </returns>
    public Vector3 UpdateMove(float deltaTime, Vector2 moveInput, ref Vector3 currentVelocity)
    {
        // Get the correct speed, account for crouching later.
        float speed = walkSpeed;

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

        // Apply speed, return.
        return currentVelocity * deltaTime;
    }
}
