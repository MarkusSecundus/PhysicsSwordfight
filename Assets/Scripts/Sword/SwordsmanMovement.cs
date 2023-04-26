using MarkusSecundus.PhysicsSwordfight.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Op;


public class SwordsmanMovement : MonoBehaviour
{
    [System.Serializable]
    public struct KeyPair
    {
        public KeyCode Increment, Decrement;
    }

    [System.Serializable]
    public class InputMapping
    {
        public KeyCode Jump = KeyCode.LeftShift;
        public InputAxis WalkForwardBackward = InputAxis.Vertical;
        public InputAxis StrafeLeftRight = InputAxis.Horizontal;
        public InputAxis LookUpDown = InputAxis.MouseY;
        public InputAxis LookLeftRight = InputAxis.MouseX;
        public InputAxis RotateLeftRight = InputAxis.HorizontalSecondary;
    }
    [System.Serializable]
    public class InputMultipliers
    {
        public float WalkForwardBackward = 1f;
        public float StrafeLeftRight = 1f;
        public float WalkMidairMultiplier = 0.5f;

        public float RotateLeftRight = 1f;
        public float RotateLeftRightDecellerateThreshold = Mathf.Epsilon;
        public float RotateLeftRightStabilizationRate = 1f;

        public float LookUpDown = 1f;
        public float LookLeftRight = 0.1f;
        public Vector3Interval LookConstraints;

        public Vector3 Jump = Vector3.up;
        
        public Vector3 Gravity = new Vector3(0, -9.81f, 0);
        public Vector3 GravityWhenGrounded = new Vector3(0, -9.81f, 0);
    }

    private static class Constants
    {
        public const ForceMode GravityMode = ForceMode.Acceleration;
        public const ForceMode JumpMode = ForceMode.VelocityChange;
    }


    public ISwordInput Input;
    public Transform CameraToUse;
    public TriggerActivityInfo Feet;

    private new Rigidbody rigidbody;

    public InputMapping Mapping = new InputMapping();
    public InputMultipliers Tweaks = new InputMultipliers();


    public readonly struct MovementDirectionBasesList
    {
        readonly SwordsmanMovement self;
        public MovementDirectionBasesList(SwordsmanMovement self) => this.self = self;

        public Vector3 WalkForwardBackwardBase => self.transform.forward;
        public Vector3 StrafeLeftRightBase => self.transform.right;
        public Vector3 RotateLeftRightAxis => self.transform.up;
    }
    public MovementDirectionBasesList MovementDirectionBases => new MovementDirectionBasesList(this);


    void Start()
    {
        Input = Input.IfNil(ISwordInput.Get(this.gameObject));
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
    }

    private void Update()
    {
        DoHandleInputs(Time.deltaTime);
    }
    void FixedUpdate()
    {
        DoMoveStep(Time.fixedDeltaTime);
    }
    bool IsGrounded => Feet.ActiveNormalCollidersCount > 0;
    




    void DoHandleInputs(float delta)
    {
        HandleWalkingInputs(delta);
        HandleRotatingInputs(delta);
        HandleLookingInputs(delta);
        HandleJumpingInputs(delta);
    }
    void DoMoveStep(float delta)
    {
        HandleWalking(delta);
        HandleRotating(delta);
        HandleLooking(delta);
        HandleJumping(delta);
        HandleGravity(delta);
    }

    void HandleWalkingInputs(float delta) { }
    void HandleWalking(float delta)
    {
        var walkForward = Input.GetAxis(Mapping.WalkForwardBackward) * Tweaks.WalkForwardBackward;
        var strafeLeftRight = Input.GetAxis(Mapping.StrafeLeftRight) * Tweaks.StrafeLeftRight;

        var toMove = (walkForward*MovementDirectionBases.WalkForwardBackwardBase + strafeLeftRight* MovementDirectionBases.StrafeLeftRightBase); //*delta !!velocity should not be multiplied by delta (opposed to applied force)!

        rigidbody.MoveToVelocity(toMove.With(y: toMove.y + rigidbody.velocity.y));
    }





    void HandleRotatingInputs(float delta) { }
    void HandleRotating(float delta)
    {
        float toRotate = Input.GetAxis(Mapping.RotateLeftRight);

        rigidbody.MoveRotation(rigidbody.rotation.WithEuler(x: 0f, z: 0f)); //?! (because the axis locking doesn't actaully work 100%) - for reason see docs ( https://docs.unity3d.com/2022.2/Documentation/ScriptReference/Rigidbody-inertiaTensor.html )

        var rotateLeftRight = toRotate *delta  * Tweaks.RotateLeftRight * MovementDirectionBases.RotateLeftRightAxis;
        rigidbody.MoveToAngularVelocity(rotateLeftRight);
    }



    void HandleLookingInputs(float delta) { }
    void HandleLooking(float delta)
    {
        if (CameraToUse == null) return;

        float lookLeftRight = Input.GetAxis(Mapping.LookLeftRight) * Tweaks.LookLeftRight * delta; //here delta makes sense since we are manually moving the camera rotation by some ammount
        float lookUpDown = Input.GetAxis(Mapping.LookUpDown) * Tweaks.LookUpDown * delta;


        var currentCameraRotation = CameraToUse.localRotation.eulerAngles;
        var xRotation = currentCameraRotation.x - lookUpDown;   //for some reason needs to use substraction to not be inverted
        var yRotation = currentCameraRotation.y + lookLeftRight; //no need to clamp this to the reasonable <0;180°> interval - that is taken care of just by forcing the z rotation to 0° (if the player managed to force y outside of this range, that changes z to upside-down a.k.a 180° - reseting that back to 0° by lucky coincidence resets the camera exactly to the reasonable position that one would want in such scenario)
        var rotationToSet = new Vector3(xRotation, yRotation, 0f).ClampEuler(Tweaks.LookConstraints);   
        if (CameraToUse != null)
            CameraToUse.localRotation = Quaternion.Euler(rotationToSet);
    }




    bool shouldJump = false;
    void HandleJumpingInputs(float delta) 
    {
        if (Input.GetKeyDown(Mapping.Jump))
            shouldJump = true;
    }
    void HandleJumping(float delta)
    {
        if (post_assign(ref shouldJump, false) && IsGrounded)
            rigidbody.AddRelativeForce(Tweaks.Jump, Constants.JumpMode);
    }

    void HandleGravity(float delta)
    {
        rigidbody.AddRelativeForce(
          IsGrounded
            ? Tweaks.GravityWhenGrounded
            : Tweaks.Gravity
        , Constants.GravityMode);
    }
}

