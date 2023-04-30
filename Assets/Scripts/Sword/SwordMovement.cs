using UnityEngine;

using MarkusSecundus.PhysicsSwordfight.Utils.Interpolation;
using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.Submodules;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.PhysicsUtils;

using HoldingForceInterpolator = MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator<float, MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator.FloatInterpolationPolicy>;
using ConnectedAnchorInterpolator = MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator<UnityEngine.Vector3, MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator.VectorInterpolationPolicy>;


namespace MarkusSecundus.PhysicsSwordfight.Sword
{
    /// <summary>
    /// Component responsible for controlling the sword.
    /// 
    /// State machine - the individual states are derived from <see cref="SwordMovement.Module"/>.
    /// 
    /// Component itself takes care of low-level movement of the sword in all 6 degrees of freedom - instructions where the sword should move to are provided by the currently active state.
    /// 
    /// Requires components <see cref="Rigidbody"/>, <see cref="ConfigurableJoint"/> and <see cref="SwordDescriptor"/>.
    /// </summary>
    [RequireComponent(typeof(ConfigurableJoint)), RequireComponent(typeof(SwordDescriptor))]
    public class SwordMovement : MonoBehaviour, ISwordMovement
    {
        /// <summary>
        /// Class representing one of the states of sword movement control.
        /// </summary>
        [System.Serializable] public abstract class Module : IScriptSubmodule<ISwordMovement> { }

        /// <summary>
        /// Descriptor of the sword that's being controlled
        /// </summary>
        public SwordDescriptor Sword { get; private set; }
        /// <summary>
        /// Source of user input.
        /// </summary>
        public ISwordInput Input { get; private set; }

        /// <summary>
        /// Joint that connects the sword with the wielder
        /// </summary>
        private ConfigurableJoint joint;

        /// <summary>
        /// Transform of the swordsman who's holding the sword. When recording sword instructions for replay, they should be converted from world coords to coords relative to the SwordWielder.
        /// </summary>
        public Transform SwordWielder => joint.connectedBody.transform;

        void Start()
        {
            Input = ISwordInput.Get(this.gameObject);
            Sword = GetComponent<SwordDescriptor>();
            joint = GetComponent<ConfigurableJoint>();

            InitSwordMovers();
            InitModes();
        }

        #region Modes
        /// <summary>
        /// Container for the modules. Contains one default module and <see cref="KeyCode"/>-indexed dictionary containing the others.
        /// </summary>
        public ScriptSubmodulesContainer<KeyCode, Module, ISwordMovement> Modes;
        IScriptSubmodule<ISwordMovement> submoduleManager;
        void InitModes()
        {
            submoduleManager = new ScriptSubmoduleListManager<KeyCode, Module, ISwordMovement>() { ActivityPredicate = Input.GetKey, ModesSupplier = () => Modes }.Init(this);
        }
        void Update() => submoduleManager.OnUpdate(Time.deltaTime);
        void FixedUpdate() => submoduleManager.OnFixedUpdate(Time.fixedDeltaTime);
        void OnDrawGizmos() => submoduleManager?.OnDrawGizmos();
        #endregion


        #region SwordMovement

        [SerializeField] HandleTweaker HandleTweaks;
        [System.Serializable]
        public class HandleTweaker
        {
            public RetargetableInterpolator.Config HandleMovementInterpolationConfig = new RetargetableInterpolator.Config { InterpolationFactor = 9f };
            public RetargetableInterpolator.Config HandleForceInterpolationConfig = new RetargetableInterpolator.Config { InterpolationFactor = 9f };
            public float DamperFactor = 0.01f;
        }

        JointRotationHelper jointRotationHelper;
        ConnectedAnchorInterpolator connectedAnchorPositioner;
        HoldingForceInterpolator holdingForceSetter;
        private void InitSwordMovers()
        {
            jointRotationHelper = joint.MakeRotationHelper();

            //Setup setter for connectedAnchor
            {
                joint.autoConfigureConnectedAnchor = false;
                StartCoroutine(connectedAnchorPositioner = new ConnectedAnchorInterpolator
                {
                    ToYield = new WaitForFixedUpdate(),
                    DeltaTimeGetter = () => Time.fixedDeltaTime,
                    Getter = () => this.joint.connectedAnchor,
                    Setter = value => this.joint.connectedAnchor = value,
                    Config = this.HandleTweaks.HandleMovementInterpolationConfig,
                    Target = this.joint.connectedAnchor
                });
            }

            //Setup setter for holding force
            {
                var originalSlerpDrive = this.joint.slerpDrive;
                var holdingForceRatio = 1f;
                holdingForceSetter = new HoldingForceInterpolator
                {
                    ToYield = new WaitForFixedUpdate(),
                    DeltaTimeGetter = () => Time.fixedDeltaTime,
                    Getter = () => holdingForceRatio,
                    Setter = value =>
                    {
                        holdingForceRatio = value;
                        var drive = originalSlerpDrive;
                        drive.positionSpring += drive.positionSpring * value;
                        drive.positionDamper += drive.positionDamper * (this.HandleTweaks.DamperFactor * value);
                        this.joint.slerpDrive = drive;
                        //Debug.Log($"target({holdingForceSetter.Target}), dr({value}), spring({originalSlerpDrive.positionSpring} => {drive.positionSpring} => {this.Joint.slerpDrive.positionSpring})", this);
                    },
                    Config = this.HandleTweaks.HandleForceInterpolationConfig,
                    Target = holdingForceRatio
                };
                StartCoroutine(holdingForceSetter);
            }
        }



        /// <summary>
        /// Set target position to which the sword will move over time.
        /// </summary>
        /// <param name="m">Where the sword should move to</param>
        public void MoveSword(ISwordMovement.MovementCommand m)
        {
            Vector3 anchor = m.AnchorPoint;
            MoveAnchorPosition(anchor);

            Vector3 up = m.UpDirection ?? computeUpVector(m.LookDirection, anchor);
            SetSwordRotation(Quaternion.LookRotation(m.LookDirection, up));

            MoveHoldingForce(m.HoldingForce);

            Vector3 computeUpVector(Vector3 lookAt, Vector3 anchor) => Vector3.up;
        }


        private void SetSwordRotation(Quaternion rotation)
        {
            jointRotationHelper.SetTargetRotation(Quaternion.Inverse(SwordWielder.transform.rotation) * rotation);
        }
        private void MoveAnchorPosition(Vector3 absolutePosition)
        {
            var relative = SwordWielder.transform.GlobalToLocal(absolutePosition);
            connectedAnchorPositioner.Target = relative;
        }
        private void MoveHoldingForce(float multiplier) => holdingForceSetter.Target = multiplier;

        #endregion

        /// <summary>
        /// Destroys this conpoment and all the other machinery that makes the sword be held in swordsman's hands - the sword will start to be affected by gravity and drop to the ground.
        /// </summary>
        public void DropTheSword()
        {
            GetComponent<Rigidbody>().useGravity = true;
            transform.SetParent(null);
            Destroy(joint);
            Destroy(this);
        }
    }


    /// <summary>
    /// Public interface of the <see cref="SwordMovement"/> component. Its instance is injected into instances of <see cref="SwordMovement.Module"/>.
    /// </summary>
    public interface ISwordMovement
    {
        /// <summary>
        /// Descriptor of the sword that's being controlled
        /// </summary>
        public SwordDescriptor Sword { get; }

        /// <summary>
        /// Source of user input.
        /// </summary>
        public ISwordInput Input { get; }

        /// <summary>
        /// Transform of the swordsman who's holding the sword. When recording sword instructions for replay, they should be converted from world coords to coords relative to the SwordWielder.
        /// </summary>
        public Transform SwordWielder { get; }

        /// <summary>
        /// Struct describing position in space that the sword should target.
        /// </summary>
        [System.Serializable]
        public struct MovementCommand
        {
            /// <summary>
            /// Vector describing the direction in world space (relative to (0,0,0)) where the sword should point to.
            /// </summary>
            public Vector3 LookDirection;
            /// <summary>
            /// Vector describing the target position of the sword's anchor (aka. the point where the sword is held) in world space.
            /// </summary>
            public Vector3 AnchorPoint;
            /// <summary>
            /// Vector describing the direction up (for purposes of computing the sword's target rotation as with <see cref="Quaternion.LookRotation(Vector3, Vector3)"/>) in world space
            /// </summary>
            public Vector3? UpDirection;
            /// <summary>
            /// How many times should increase the force with which the sword is held. If 0, default force will be used. 
            /// </summary>
            public float HoldingForce;

            public override string ToString()
                => $"{{look{LookDirection}, anchor{AnchorPoint}, up{UpDirection}}}";
        }

        /// <summary>
        /// Set target position to which the sword will move over time.
        /// </summary>
        /// <param name="m">Where the sword should move to</param>
        public void MoveSword(MovementCommand m);
    }
    /// <summary>
    /// Static class containing convenient extension methods for <see cref="ISwordMovement"/>.
    /// </summary>
    public static class SwordMovementExtensions
    {
        /// <summary>
        /// Set target position to which the sword will move over time.
        /// </summary>
        /// <param name="self">Instance to be used as <c>this</c></param>
        /// <param name="lookDirection">
        /// Vector describing the direction in world space (relative to (0,0,0)) where the sword should point to.
        /// </param>
        /// <param name="anchorPoint">
        /// Vector describing the target position of the sword's anchor (aka. the point where the sword is held) in world space.</param>
        /// <param name="upDirection">
        /// Vector describing the direction up (for purposes of computing the sword's target rotation as with <see cref="Quaternion.LookRotation(Vector3, Vector3)"/>) in world space
        /// </param>
        /// <param name="holdingForce">
        /// How many times should increase the force with which the sword is held. If 0, default force will be used. 
        /// </param>
        public static void MoveSword(this ISwordMovement self, Vector3 lookDirection, Vector3 anchorPoint, Vector3? upDirection = null, float holdingForce = 0f) => self.MoveSword(new ISwordMovement.MovementCommand { LookDirection = lookDirection, AnchorPoint = anchorPoint, UpDirection = upDirection, HoldingForce = holdingForce });
    }

}