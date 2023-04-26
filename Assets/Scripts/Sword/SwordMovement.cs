using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Linq;

using HoldingForceInterpolator = MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator<float, MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator.FloatInterpolationPolicy>;
using ConnectedAnchorInterpolator = MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator<UnityEngine.Vector3, MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator.VectorInterpolationPolicy>;
using UnityEngine.Events;
using Newtonsoft.Json;
using MarkusSecundus.PhysicsSwordfight.Utils.Interpolation;
using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.Submodules;

public class SwordMovement : MonoBehaviour, ISwordMovement
{
    [System.Serializable] public abstract class Submodule : IScriptSubmodule<ISwordMovement>{}

    [field: SerializeField] public SwordDescriptor Sword { get; private set; }
    [field: SerializeField] public ConfigurableJoint Joint { get; private set; }
    [field: SerializeField] public ISwordInput Input { get; private set; }

    public Transform SwordWielder => Joint.connectedBody.transform;

    void Start()
    {
        Input = Input.IfNil(ISwordInput.Get(this.gameObject));
        Sword = Sword.IfNil(GetComponent<SwordDescriptor>());
        Joint = Joint.IfNil(GetComponent<ConfigurableJoint>());

        InitSwordMovers();
        InitModes();
    }

    #region Modes
    public ScriptSubmodulesContainer<KeyCode, Submodule, ISwordMovement> Modes;
    IScriptSubmodule<ISwordMovement> submoduleManager;
    void InitModes()
    {
        submoduleManager = new ScriptSubmoduleListManager<KeyCode, Submodule, ISwordMovement>() { ActivityPredicate = Input.GetKey, ModesSupplier = ()=>Modes }.Init(this);
    }
    void Update() => submoduleManager.OnUpdate(Time.deltaTime);
    void FixedUpdate() => submoduleManager.OnFixedUpdate(Time.fixedDeltaTime);
    void OnDrawGizmos() => submoduleManager?.OnDrawGizmos();
    #endregion


    #region SwordMovement

    [SerializeField] HandleTweaker HandleTweaks;
    [System.Serializable] public class HandleTweaker
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
        jointRotationHelper = Joint.MakeRotationHelper();

        //Setup setter for connectedAnchor
        {
            Joint.autoConfigureConnectedAnchor = false;
            StartCoroutine(connectedAnchorPositioner = new ConnectedAnchorInterpolator
            {
                ToYield = new WaitForFixedUpdate(),
                DeltaTimeGetter = () => Time.fixedDeltaTime,
                Getter = () => this.Joint.connectedAnchor,
                Setter = value => this.Joint.connectedAnchor = value,
                Config = this.HandleTweaks.HandleMovementInterpolationConfig,
                Target = this.Joint.connectedAnchor
            });
        }

        //Setup setter for holding force
        {
            var originalSlerpDrive = this.Joint.slerpDrive;
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
                    drive.positionSpring += drive.positionSpring*value;
                    drive.positionDamper += drive.positionDamper * (this.HandleTweaks.DamperFactor * value);
                    this.Joint.slerpDrive = drive;
                    //Debug.Log($"target({holdingForceSetter.Target}), dr({value}), spring({originalSlerpDrive.positionSpring} => {drive.positionSpring} => {this.Joint.slerpDrive.positionSpring})", this);
                },
                Config = this.HandleTweaks.HandleForceInterpolationConfig,
                Target = holdingForceRatio
            };
            StartCoroutine(holdingForceSetter);
        }
    }



    public void MoveSword(ISwordMovement.MovementCommand m)
    {
        Vector3 anchor = m.AnchorPoint;
        MoveAnchorPosition(anchor);

        Vector3 up = m.UpDirection??computeUpVector(m.LookDirection, anchor);
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

    public void DropTheSword()
    {
        GetComponent<Rigidbody>().useGravity = true;
        transform.SetParent(null);
        Destroy(Joint);
        Destroy(this);
    }
}



public interface ISwordMovement
{
    public SwordDescriptor Sword { get;  }
    public ISwordInput Input { get; }
    public Transform SwordWielder { get; }

    [System.Serializable]
    public struct MovementCommand
    {
        public Vector3 LookDirection;
        public Vector3 AnchorPoint;
        public Vector3? UpDirection;
        public float HoldingForce;

        public override string ToString()
            => $"{{look{LookDirection}, anchor{AnchorPoint}, up{UpDirection}}}";
    }
    public void MoveSword(MovementCommand m);
}
public static class SwordMovementExtensions
{
    public static void MoveSword(this ISwordMovement self, Vector3 lookDirection, Vector3 anchorPoint, Vector3? upDirection = null, float holdingForce=0f) => self.MoveSword(new ISwordMovement.MovementCommand { LookDirection = lookDirection, AnchorPoint = anchorPoint, UpDirection = upDirection, HoldingForce = holdingForce });
}
