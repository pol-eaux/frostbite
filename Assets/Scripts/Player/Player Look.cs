using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] float sensitivity = 10f;
    [SerializeField] float minViewAngle = -90f;
    [SerializeField] float maxViewAngle = 90f;

    private float _xRotation = 0f;
    private float _yRotation = 0f;

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

        // Apply rotation.
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }


    /// <returns> An update y rotation float value. </returns>
    public float GetYRotation() => _yRotation;
}
