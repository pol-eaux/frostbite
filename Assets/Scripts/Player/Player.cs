using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;

    private PlayerInputActions _inputActions;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }

    void Update()
    {
        var input = _inputActions.Gameplay;

        // Get camera input and update its rotation.
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
        playerCamera.UpdateRotation(cameraInput);

        // Get character input and update it.
        var characterInput = new CharacterInput
        { 
            Rotation = playerCamera.transform.rotation, 
            Move = input.Move.ReadValue<Vector2>()
        };
        playerCharacter.UpdateInput(characterInput);
    }
}
