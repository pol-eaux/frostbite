using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Constants
    const float GRAVITY = -9.81f;

    // Input System
    public InputActionAsset InputActions;
    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private Vector2 m_moveAmount;

    // Private Components
    private CharacterController m_characterController;

    [Header("Movement Settings")]
    [SerializeField]
    private float _walkSpeed = 5;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 5;
    private float _verticalVelocity;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");

        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        m_moveAmount = m_moveAction.ReadValue<Vector2>();

        ProcessJump();
    }

    private void FixedUpdate()
    {
        ProcessWalking();
    }

    private void ProcessJump()
    {
        if (m_jumpAction.WasPressedThisFrame() && m_characterController.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * GRAVITY);
        }
    }

    private void ProcessWalking()
    {
        if(m_characterController.isGrounded && _verticalVelocity < 0)
        {
            // Make vertical velocity a small negative number to keep grounded.
            _verticalVelocity = -2f;
        }

        Vector3 moveDirection = transform.forward * m_moveAmount.y + transform.right * m_moveAmount.x;

        // Prevent faster diagonal movement.
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Apply gravity and upward movement.
        _verticalVelocity += GRAVITY * Time.deltaTime;
        moveDirection.y = _verticalVelocity;

        m_characterController.Move(moveDirection * _walkSpeed * Time.deltaTime);
    }
}
