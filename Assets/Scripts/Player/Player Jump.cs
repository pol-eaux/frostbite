using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Tooltip("How quickly the player falls back to the ground.")]
    [SerializeField] private float gravity = -9.81f;
    [Tooltip("How hight the player jumps.")]
    [SerializeField] private float jumpForce = 5f;
    [Tooltip("Maximum falling speed.")]
    [SerializeField] private float terminalVelocity = -50f;

    private float _verticalVelocity;

    /// <summary>
    /// Calculates a float that will be the vertical velocity for the player.
    /// </summary>
    /// <param name="deltaTime"> Global delta time. </param>
    /// <param name="isJumping"> Jump input. </param>
    /// <param name="isGrounded"> Character grounding. </param>
    /// <returns></returns>
    public float UpdateGravity(float deltaTime, bool isJumping, bool isGrounded, bool isCrouching)
    {
        // If the player is grounded.
        if(isGrounded)
        {
            // If the player jump button pressed and the player is not crouching, jump.
            if(isJumping && !isCrouching)
            {
                _verticalVelocity = jumpForce;
            }
            // Otherwise apply a small force to keep the player grounded.
            else
            {
                 if (_verticalVelocity < 0)
                 {
                    _verticalVelocity = -2f;
                 }
            }
        }
        // If the player is in the air, apply gravity.
        else
        {
            _verticalVelocity += gravity * deltaTime;

            // If the player falls for a long time, hard set the vertical velocity to a set terminal velocity.
            if(_verticalVelocity < terminalVelocity)
            {
                _verticalVelocity = terminalVelocity;
            }
        }

        return _verticalVelocity;
    }
}
