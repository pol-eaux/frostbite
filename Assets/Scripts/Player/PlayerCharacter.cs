using KinematicCharacterController;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum CrouchInput
{
    None,
    Toggle
}

public enum Stance
{
    Stand,
    Crouch
}

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector3 Move;
    public bool Jump;
    public CrouchInput Crouch;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform cameraTarget;
    [Space]

    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float crouchSpeed = 10f;
    [SerializeField] private float walkResponse = 25f;
    [SerializeField] private float crouchResponse = 20f;
    [Space]

    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float gravity = -90f;
    [Space]

    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchHeightResponse = 15f;
    [SerializeField, Range(0f, 1f)] private float standCameraTargetHeight = 1f;
    [SerializeField, Range(0f, 1f)] private float crouchCameraTargetHeight = 0.7f;

    private Stance _stance;
    private Collider[] _uncrouchOverlapResults;

    private Quaternion _requestedRotation;
    [HideInInspector] public Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedCrouch;

    public void Initialize()
    {
        _stance = Stance.Stand;
        _uncrouchOverlapResults = new Collider[8];
        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.Rotation;
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        // Prevent faster diagonal movement.
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);

        _requestedMovement = input.Rotation * _requestedMovement;

        // Request the jump if it's already being requested or has just been requested.
        _requestedJump = _requestedJump || input.Jump;
        // Request crouch depending on the type of input, toggle or hold.
        _requestedCrouch = input.Crouch switch
        {
            CrouchInput.Toggle => !_requestedCrouch,
            CrouchInput.None => _requestedCrouch,
            _ => _requestedCrouch
        };
    }

    public void UpdateBody(float deltaTime)
    {
        var currentHeight = motor.Capsule.height;
        var normalizedHeight = currentHeight / standHeight;
        var cameraTargetHeight = currentHeight * (_stance is Stance.Stand ? standCameraTargetHeight : crouchCameraTargetHeight);
        var rootTargetScale = new Vector3(1f, normalizedHeight, 1f);

        cameraTarget.localPosition = Vector3.Lerp(a: cameraTarget.localPosition, b: new Vector3(0f, cameraTargetHeight, 0f), t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime));
        root.localScale = Vector3.Lerp(a: root.localScale, b: rootTargetScale, t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime));
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        // Uncrouch the player.
        if (!_requestedCrouch && _stance is not Stance.Stand)
        {
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: standHeight,
                yOffset: standHeight * 0.5f
            );

            // If the player collides with anything, keep crouching.
            if (motor.CharacterOverlap
                (motor.TransientPosition,
                 motor.TransientRotation,
                 _uncrouchOverlapResults,
                 motor.CollidableLayers,
                 QueryTriggerInteraction.Ignore
                ) > 0
            )
            {
                _requestedCrouch = true;
                motor.SetCapsuleDimensions
                (
                    radius: motor.Capsule.radius,
                    height: crouchHeight,
                    yOffset: crouchHeight * 0.5f
                );
            }
            // Else the player can remain standing.
            else
            {
                _stance = Stance.Stand;
            }
        }
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        // Crouch the player.
        if (_requestedCrouch && _stance is Stance.Stand)
        {
            _stance = Stance.Crouch;
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
            );
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane
        (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
        );

        if (forward != Vector3.zero )
        {
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // If on the ground:
        if(motor.GroundingStatus.IsStableOnGround)
        {
            // Snap the requested movement direction to the angle of the surface the character is on.
            var groundedMovement = motor.GetDirectionTangentToSurface
            (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            // Determine speed and response based on stance.
            var speed = _stance is Stance.Stand ? walkSpeed : crouchSpeed;
            var response = _stance is Stance.Stand ? walkResponse : crouchResponse;

            // Move along the ground in that direction.
            var targetVelocity = groundedMovement * speed;
            currentVelocity = Vector3.Lerp(a: currentVelocity, b: targetVelocity, t: 1f - Mathf.Exp(-response * deltaTime));
        }
        // Else, in the air:
        else
        {
            // Add gravity.
            currentVelocity += motor.CharacterUp * gravity * deltaTime;
        }

        // If a jump is requested and the player is grounded:
        if(_requestedJump && motor.GroundingStatus.IsStableOnGround)
        {
            _requestedJump = false;

            // Unstick the player from the ground.
            motor.ForceUnground(time: 0.1f);

            // Add a jump force.
            currentVelocity += motor.CharacterUp * jumpSpeed;
        }

    }

    public Transform GetCameraTarget() => cameraTarget;
}
