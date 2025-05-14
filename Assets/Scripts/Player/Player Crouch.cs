using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float transitionSpeed = 6f;

    private float _currentHeight;
    private float _targetHeight;
}
