using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    // Input System
    public InputActionAsset InputActions;
    private Vector2 m_lookAmount;
    private InputAction m_lookAction;

    // Private Components
    private Transform m_cameraTransform;

    [Header("Look Settings")]
    [SerializeField] private float _sensitivityX = 1;
    [SerializeField] private float _sensitivityY = 1;
    [SerializeField] private float _maxPitch = 90;
    [SerializeField] private float _minPitch = -90;
    private float _pitch = 0f;

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
        m_lookAction = InputSystem.actions.FindAction("Look");

        m_cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        m_lookAmount = m_lookAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        ProcessRotation();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ProcessRotation()
    {
        float yaw = m_lookAmount.x * _sensitivityX * Time.deltaTime;
        transform.Rotate(0, yaw, 0);

        float pitchDelta = -m_lookAmount.y * _sensitivityY * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch + pitchDelta, _minPitch, _maxPitch);

        m_cameraTransform.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }
}
