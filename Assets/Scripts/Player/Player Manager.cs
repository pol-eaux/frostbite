using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

/// <summary>
/// Class <c>PlayerManager</c>
/// This class in responsible for getting inputs from the input system and delegating 
/// necessary information to other player scripts.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerLook playerLook;

    private CharacterController _characterController;

    // Input Actions.
    private InputAction _moveAction;
    private InputAction _lookAction;

    // Inputs from the input system.
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private bool _isGrounded;

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

        // Get character controller.
        _characterController = GetComponent<CharacterController>();
        _isGrounded = _characterController.isGrounded;
    }

    private void Start()
    {
        // Lock and hide the cursor.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        _isGrounded = _characterController.isGrounded;

        // Get inputs for this frame.
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();

        // Update Player scripts:
        // Move the character controller by calling the player movement script and getting back a Vector3.
        _characterController.Move(playerMovement.UpdateMove(deltaTime, _moveInput, ref currentVelocity));
        // Rotate the player camera along the x-axis by calling the update function in the player look script.
        playerLook.UpdateLook(deltaTime, _lookInput);
        // Rotate the player object along the y-axis by getting the y-rotation calculated in the update look funciton.
        transform.localRotation = Quaternion.Euler(0f, playerLook.GetYRotation(), 0f);
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Gameplay").Disable();
    }
}
