using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.PhysicsUtils;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MarkusSecundus.Utils.Op;


namespace MarkusSecundus.PhysicsSwordfight.Sword
{
    /// <summary>
    /// Component responsible for swordsman's movement. Allows normal walking forwards/backwards and sideways + looking left/right, up/down.
    /// </summary>
    [Tooltip("Component responsible for swordsman's movement. Allows normal walking forwards/backwards and sideways + looking left/right, up/down")]
    [RequireComponent(typeof(Rigidbody))]
    public class SwordsmanMovement : MonoBehaviour
    {
        /// <summary>
        /// Container defining input mapping
        /// </summary>
        [Tooltip("Container defining input mapping")]
        [System.Serializable]
        public class InputMapping
        {
            /// <summary>
            /// Key to initiate jump
            /// </summary>
            [Tooltip("Key to initiate jump")]
            public KeyCode Jump = KeyCode.LeftShift;
            /// <summary>
            /// Axis for walking in backward/forward direction
            /// </summary>
            [Tooltip("Axis for walking in backward/forward direction")]
            public InputAxis WalkForwardBackward = InputAxis.Vertical;
            /// <summary>
            /// Axis for walking in left/right direction
            /// </summary>
            [Tooltip("Axis for walking in left/right direction")]
            public InputAxis StrafeLeftRight = InputAxis.Horizontal;
            /// <summary>
            /// Axis for rotating camera up/down
            /// </summary>
            [Tooltip("Axis for rotating camera up/down")]
            public InputAxis LookUpDown = InputAxis.MouseY;
            /// <summary>
            /// Axis for rotating camera left/right
            /// </summary>
            [Tooltip("Axis for rotating camera left/right")]
            public InputAxis LookLeftRight = InputAxis.MouseX;
            /// <summary>
            /// Axis for rotating the swordsman's body left/right
            /// </summary>
            [Tooltip("Axis for rotating the swordsman's body left/right")]
            public InputAxis RotateLeftRight = InputAxis.HorizontalSecondary;
        }
        /// <summary>
        /// Container defining parameters for movement speed and stuff
        /// </summary>
        [Tooltip("Container defining parameters for movement speed and stuff")]
        [System.Serializable]
        public class InputMultipliers
        {
            /// <summary>
            /// Speed of walking backward/forward
            /// </summary>
            [Tooltip("Speed of walking backward/forward")]
            public float WalkForwardBackward = 1f;
            /// <summary>
            /// Speed of walking left/right
            /// </summary>
            [Tooltip("Speed of walking left/right")]
            public float StrafeLeftRight = 1f;
            /// <summary>
            /// Speed of rotating swordsman's body left/right
            /// </summary>
            [Tooltip("Speed of rotating swordsman's body left/right")]
            public float RotateLeftRight = 1f;

            /// <summary>
            /// Speed of rotating camera up/down
            /// </summary>
            [Tooltip("Speed of rotating camera up/down")]
            public float LookUpDown = 1f;
            /// <summary>
            /// Speed of rotating camera left/right
            /// </summary>
            [Tooltip("Speed of rotating camera left/right")]
            public float LookLeftRight = 0f;
            /// <summary>
            /// Min and max constraints of the camera's rotation
            /// </summary>
            [Tooltip("Min and max constraints of the camera's rotation")]
            public Interval<Vector3> LookConstraints = new Interval<Vector3>(new Vector3(-180f, 0,0), new Vector3(180f, 0,0));

            /// <summary>
            /// Force impulse to apply when jumping
            /// </summary>
            [Tooltip("Force impulse to apply when jumping")]
            public Vector3 Jump = Vector3.up;
            /// <summary>
            /// Force to be applied as gravity when in midair (cannot jump)
            /// </summary>
            [Tooltip("Force to be applied as gravity when in midair (cannot jump)")]
            public Vector3 Gravity = new Vector3(0, -9.81f, 0);
            /// <summary>
            /// Force to be applied as gravity when grounded (can jump)
            /// </summary>
            [Tooltip("Force to be applied as gravity when grounded (can jump)")]
            public Vector3 GravityWhenGrounded = new Vector3(0, -9.81f, 0);
        }

        static class Constants
        {
            public const ForceMode GravityMode = ForceMode.Acceleration;
            public const ForceMode JumpMode = ForceMode.VelocityChange;
        }

        /// <summary>
        /// Input provider to be used
        /// </summary>
        [Tooltip("Input provider to be used")]
        public ISwordInput Input;
        /// <summary>
        /// Camera to be rotated
        /// </summary>
        [Tooltip("Camera to be rotated")]
        public Transform CameraToUse;
        /// <summary>
        /// Trigger collider - IFF triggered, swordsman is considered as grounded and can jump
        /// </summary>
        [Tooltip("Trigger collider - IFF triggered, swordsman is considered as grounded and can jump")]
        public TriggerActivityInfo Feet;

        private new Rigidbody rigidbody;
        /// <summary>
        /// Container defining input mapping
        /// </summary>
        [Tooltip("Container defining input mapping")]
        public InputMapping Mapping = new InputMapping();
        /// <summary>
        /// Container defining parameters for movement speed and stuff
        /// </summary>
        [Tooltip("Container defining parameters for movement speed and stuff")]
        public InputMultipliers Tweaks = new InputMultipliers();

        /// <summary>
        /// Container defining movement directions
        /// </summary>
        public readonly struct MovementDirectionBasesList
        {
            readonly SwordsmanMovement self;

            internal MovementDirectionBasesList(SwordsmanMovement self) => this.self = self;

            /// <summary>
            /// Direction for walking forward, in world space
            /// </summary>
            public Vector3 WalkForwardBackwardBase => self.transform.forward;
            /// <summary>
            /// Direction for walking left, in world space
            /// </summary>
            public Vector3 StrafeLeftRightBase => self.transform.right;

            /// <summary>
            /// Axis for rotating left/right, in world space
            /// </summary>
            public Vector3 RotateLeftRightAxis => self.transform.up;
        }
        /// <summary>
        /// Container defining movement directions
        /// </summary>
        public MovementDirectionBasesList MovementDirectionBases => new MovementDirectionBasesList(this);


        void Start()
        {
            Input = Input.IfNil(ISwordInput.Get(this.gameObject));
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            InitLooking();
        }

        void Update()
        {
            DoHandleInputs(Time.deltaTime);
            HandleLooking(Time.deltaTime);
        }
        void FixedUpdate()
        {
            DoFixedMoveStep(Time.fixedDeltaTime);
        }
        bool IsGrounded => Feet.ActiveNormalCollidersCount > 0;





        void DoHandleInputs(float delta)
        {
            HandleWalkingInputs(delta);
            HandleRotatingInputs(delta);
            HandleLookingInputs(delta);
            HandleJumpingInputs(delta);
        }
        void DoFixedMoveStep(float delta)
        {
            HandleWalking(delta);
            HandleRotating(delta);
            HandleJumping(delta);
            HandleGravity(delta);
        }

        void HandleWalkingInputs(float delta) { }
        void HandleWalking(float delta)
        {
            var walkForward = Input.GetAxis(Mapping.WalkForwardBackward) * Tweaks.WalkForwardBackward;
            var strafeLeftRight = Input.GetAxis(Mapping.StrafeLeftRight) * Tweaks.StrafeLeftRight;

            var toMove = (walkForward * MovementDirectionBases.WalkForwardBackwardBase + strafeLeftRight * MovementDirectionBases.StrafeLeftRightBase); //*delta !!velocity should not be multiplied by delta (opposed to applied force)!

            rigidbody.MoveToVelocity(toMove.With(y: toMove.y + rigidbody.velocity.y));
        }





        void HandleRotatingInputs(float delta) { }
        void HandleRotating(float delta)
        {
            float toRotate = Input.GetAxis(Mapping.RotateLeftRight);

            rigidbody.MoveRotation(rigidbody.rotation.WithEuler(x: 0f, z: 0f)); //?! (because the axis locking doesn't actaully work 100%) - for reason see docs ( https://docs.unity3d.com/2022.2/Documentation/ScriptReference/Rigidbody-inertiaTensor.html )

            var rotateLeftRight = toRotate * delta * Tweaks.RotateLeftRight * MovementDirectionBases.RotateLeftRightAxis;
            rigidbody.MoveToAngularVelocity(rotateLeftRight);
        }



        Vector3 cameraRotation;
        void InitLooking()
        {
            cameraRotation = CameraToUse.transform.rotation.eulerAngles;
        }
        void HandleLookingInputs(float delta) { }
        void HandleLooking(float delta)
        {
            if (CameraToUse == null) return;

            float lookLeftRight = Input.GetAxis(Mapping.LookLeftRight) * Tweaks.LookLeftRight * delta; //here delta makes sense since we are manually moving the camera rotation by some ammount
            float lookUpDown = Input.GetAxis(Mapping.LookUpDown) * Tweaks.LookUpDown * delta;


            
            var xRotation = cameraRotation.x - lookUpDown;   //for some reason needs to use substraction to not be inverted
            var yRotation = cameraRotation.y + lookLeftRight; //no need to clamp this to the reasonable <0;180°> interval - that is taken care of just by forcing the z rotation to 0° (if the player managed to force y outside of this range, that changes z to upside-down a.k.a 180° - reseting that back to 0° by lucky coincidence resets the camera exactly to the reasonable position that one would want in such scenario)
            cameraRotation = new Vector3(xRotation, yRotation, 0f).ClampEuler(Tweaks.LookConstraints);
            if (CameraToUse != null)
                CameraToUse.localRotation = Quaternion.Euler(cameraRotation);
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

}