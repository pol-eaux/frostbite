using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class <c>PlayerManager</c>
/// This class in responsible for getting inputs from the input system and delegating 
/// necessary information to other player scripts.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("When true, crouch is a toggle, when false crouch is held.")]
    [SerializeField] private bool toggleCrouch = false;

    [Header("Script Components")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerJump playerJump;
    [SerializeField] private PlayerCrouch playerCrouch;

    [Space]
    [SerializeField] private CameraLook cameraLook;
    [SerializeField] private CameraBob cameraBob;
    [SerializeField] private CameraRaycasting cameraRaycasting;

    [Header("Transforms")]
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform cameraAnchorTransform;

    private CharacterController _characterController;

    // Input Actions.
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;
    private InputAction _interactAction;

    // Inputs from the input system.
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    // Input Booleans
    // Jump input.
    private bool _jumpPressed;
    // Crouch input.
    private bool _crouchHeld;
    // Sprint input.
    private bool _sprintHeld;
    // Is the player grounded this frame?
    private bool _isGrounded;
    // Interact input.
    private bool _interactPressed;

    // State Booleans
    // Is the player crouching this frame?
    private bool _isCrouching;
    private bool _crouchInputPrevious;
    // Is the player sprinting this frame?
    private bool _isSprinting;
    // Is the player jumping this frame?
    private bool _isJumping;
    // Is the player interacting this frame?
    private bool _isInteracting;

    // For acceleration.
    private Vector3 currentVelocity = Vector3.zero;

    private void OnEnable()
    {
        inputActions.FindActionMap("Gameplay").Enable();
    }

    private void Awake()
    {
        // Get inputs.
        var gameplayMap = inputActions.FindActionMap("Gameplay");
        _moveAction = gameplayMap.FindAction("Move");
        _lookAction = gameplayMap.FindAction("Look");
        _jumpAction = gameplayMap.FindAction("Jump");
        _crouchAction = gameplayMap.FindAction("Crouch");
        _sprintAction = gameplayMap.FindAction("Sprint");
        _interactAction = gameplayMap.FindAction("Interact");

        // Get character controller.
        _characterController = GetComponent<CharacterController>();

        // Input Bools
        _isGrounded = _characterController.isGrounded;
        _jumpPressed = false;
        _crouchHeld = false;
        _sprintHeld = false;
        _interactPressed = false;

        // State Bools
        _isCrouching = false;
        _crouchInputPrevious = false;
        _isSprinting = false;
        _isJumping = false;
        _isInteracting = false;

        // Initialize component scripts.
        cameraLook.Initialize(this.transform);
        playerCrouch.Initialize(_characterController, modelTransform, cameraAnchorTransform);
        cameraBob.Initialize(_characterController);
    }

    private void Start()
    {
        // Lock and hide the cursor.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get global delta time.
        float deltaTime = Time.deltaTime;
        
        // Update all needed variables for this frame.
        UpdateVariables();

        // Call player component scripts.
        Move(deltaTime);
        Look(deltaTime);
        Crouch(deltaTime);
        Bob(deltaTime);
        Raycast(deltaTime);
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Gameplay").Disable();
    }

    /// <summary>
    /// Gets updated values for global variables needed in player component calculations.
    /// </summary>
    private void UpdateVariables()
    {
        // Is the player grounded on this frame?
        _isGrounded = _characterController.isGrounded;
        // Was jump pressed this frame?
        _jumpPressed = _jumpAction.WasPressedThisFrame();
        // Was crouch held this frame?
        _crouchHeld = _crouchAction.IsPressed();
        // Allows the player to toggle crouch but also hold the crouch button.
        bool crouchJustPressed = _crouchHeld && !_crouchInputPrevious;
        // Was sprint held this frame?
        _sprintHeld = _sprintAction.IsPressed();

        // If toggle crouch, flip value.
        if(toggleCrouch)
        {
            if(crouchJustPressed)
            {
                _isCrouching = !_isCrouching;
            }
        }
        // Else hold crouch.
        else
        {
            _isCrouching = _crouchHeld;
        }

        // Hold to sprint.
        _isSprinting = _sprintHeld;
        // Press to jump.
        _isJumping = _jumpPressed;

        // Set previous crouch input.
        _crouchInputPrevious = _crouchHeld;

        // Get interaction.
        _interactPressed = _interactAction.WasPressedThisFrame();
        _isInteracting = _interactPressed;

        // Read inputs:
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();
    }

    /// <summary>
    /// Moves the character controller by a Vector3 generated by the UpdateMove function in the PlayerMovement component.
    /// Calculates jumping and gravity by getting a float generated by the UpdateJump function in the PlayerJump component.
    /// </summary>
    /// <param name="deltaTime"> Global delta time </param>
    private void Move(float deltaTime)
    {
        // Get vertical movement (apply gravity and or jump).
        float verticalVelocity = playerJump.UpdateGravity(deltaTime, _isJumping, _isGrounded, _isCrouching);
        // Get horizontal movement.
        Vector3 horizontalMovement = playerMovement.UpdateMove(deltaTime, _moveInput, ref currentVelocity, _isCrouching, _isSprinting, _isGrounded);
        // Combine them for the final movement vector.
        Vector3 finalMovement = new Vector3(horizontalMovement.x, verticalVelocity * deltaTime, horizontalMovement.z);

        // Move the character controller.
        _characterController.Move(finalMovement);
    }

    /// <summary>
    /// Rotates the camera along the x-axis using the UpdateLook function in the PlayerLook component.
    /// Rotates this object along the y-axis by changing this objects rotation.
    /// </summary>
    /// <param name="deltaTime"> Global delta time </param>
    private void Look(float deltaTime)
    {
        // Rotate the player camera along the x-axis and this by the y-axis by calling the update look funcion in the player look component.
        cameraLook.UpdateLook(deltaTime, _lookInput);
    }

    /// <summary>
    /// Call the update stance function from the player crouch component.
    /// </summary>
    /// <param name="deltaTime"> Global delta time </param>
    private void Crouch(float deltaTime)
    {
        // Change the character controller height, center, model scale, and camera height from this funciton.
        playerCrouch.UpdateStance(deltaTime, _isCrouching, _isGrounded);
    }

    /// <summary>
    /// Call the update bob function from the camera bob component.
    /// </summary>
    /// <param name="deltaTime"> Global delta time </param>
    private void Bob(float deltaTime)
    {
        // Change the local position of the camera bob object.
        cameraBob.UpdateBob(deltaTime, _isGrounded, _isCrouching, _isSprinting);
    }

    /// <summary>
    /// Call the update raycast function from the camera raycasting component.
    /// </summary>
    /// <param name="deltaTime"> Global delta time </param>
    private void Raycast(float deltaTime)
    {
        cameraRaycasting.UpdateRaycast(deltaTime, _isInteracting);
    }
}
