using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Tooltip("How quickly the camera rotates based on input.")]
    [SerializeField] float sensitivity = 10f;
    [Tooltip("How far down the player can look.")]
    [SerializeField] float minViewAngle = -90f;
    [Tooltip("How far up the player can look.")]
    [SerializeField] float maxViewAngle = 90f;

    private float _xRotation = 0f;
    private float _yRotation = 0f;

    private Transform _playerTransform;

    /// <summary>
    /// Provide the component with reference to the player object so that
    /// it can preform y rotation.
    /// </summary>
    /// <param name="playerTransform"></param>
    public void Initialize(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }

    /// <summary>
    /// Rotates the camera along the x-axis based on input look values.
    /// Updates the y-rotation so that other scripts can rotate the player object.
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <param name="lookInput"> A Vector2 input given by the input system. </param>
    public void UpdateLook(float deltaTime, Vector2 lookInput)
    {
        // Get mouse inputs, apply sensitivity.
        float mouseX = lookInput.x * sensitivity * deltaTime;
        float mouseY = lookInput.y * sensitivity * deltaTime;

        // Rotation around the x axis (look up and down).
        _xRotation -= mouseY;

        // Clamp rotation.
        _xRotation = Mathf.Clamp(_xRotation, minViewAngle, maxViewAngle);

        // Rotation around the y axis (look left and right).
        _yRotation += mouseX;

        // Apply rotation to camera.
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        // Apply rotation to player.
        _playerTransform.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
    }
}
