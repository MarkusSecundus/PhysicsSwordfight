using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Linq;

using Interpolator = RetargetableInterpolator<UnityEngine.Vector3, RetargetableInterpolator.VectorInterpolationPolicy>;
using UnityEngine.Events;
using Newtonsoft.Json;

public class SwordMovement : MonoBehaviour, ISwordMovement
{
    [System.Serializable] public abstract class Submodule : IScriptSubmodule<ISwordMovement>{}

    [field: SerializeField] public SwordDescriptor Sword { get; private set; }
    [field: SerializeField] public ConfigurableJoint Joint { get; private set; }
    [field: SerializeField] public ISwordInput Input { get; private set; }


    void Start()
    {
        Sword = Sword.IfNil(GetComponent<SwordDescriptor>());
        Joint = Joint.IfNil(GetComponent<ConfigurableJoint>());
        Input = Input.IfNil(GetComponentInParent<ISwordInput>());

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
    [SerializeField] RetargetableInterpolator.Config HandleMovementInterpolationConfig = new RetargetableInterpolator.Config { InterpolationFactor = 9f };

    JointRotationHelper jointRotationHelper;
    Interpolator connectedAnchorPositioner;
    private void InitSwordMovers()
    {
        jointRotationHelper = Joint.MakeRotationHelper();

        Joint.autoConfigureConnectedAnchor = false;
        StartCoroutine(connectedAnchorPositioner = new Interpolator
        {
            ToYield = new WaitForFixedUpdate(),
            DeltaGetter = () => Time.deltaTime,
            Getter = () => this.Joint.connectedAnchor,
            Setter = value => this.Joint.connectedAnchor = value,
            Config = this.HandleMovementInterpolationConfig,
            Target = this.Joint.connectedAnchor
        });
    }



    public void MoveSword(ISwordMovement.MovementCommand m)
    {
        Vector3 anchor;
        if (m.AnchorPoint != null)
            MoveAnchorPosition(anchor = m.AnchorPoint.Value);
        else anchor = this.Joint.connectedBody.transform.LocalToGlobal(connectedAnchorPositioner.Target);

        Vector3 up = m.UpDirection??computeUpVector(m.LookDirection, anchor);
        SetSwordRotation(Quaternion.LookRotation(m.LookDirection, up));

        Vector3 computeUpVector(Vector3 lookAt, Vector3 anchor) => Vector3.up;
    }


    private void SetSwordRotation(Quaternion rotation) 
    {
        jointRotationHelper.SetTargetRotation(Quaternion.Inverse(Joint.connectedBody.transform.rotation) * rotation);
    }
    private void MoveAnchorPosition(Vector3 absolutePosition)
    {
        var relative = this.Joint.connectedBody.transform.GlobalToLocal(absolutePosition);
        connectedAnchorPositioner.Target = relative;
    }
    #endregion
}



public interface ISwordMovement
{
    public SwordDescriptor Sword { get;  }
    public ISwordInput Input { get; }

    [System.Serializable]
    public struct MovementCommand
    {
        [JsonIgnore] public Vector3 LookDirection;
        [JsonIgnore] public Vector3? AnchorPoint;
        [JsonIgnore] public Vector3? UpDirection;

        [JsonProperty] SerializableVector3 Look { get => LookDirection; set => LookDirection = value; }
        [JsonProperty] SerializableVector3? Anchor { get => AnchorPoint; set => AnchorPoint = value; }
        [JsonProperty] SerializableVector3? Up { get => UpDirection; set => UpDirection = value; }
    }
    public void MoveSword(MovementCommand m);
}
public static class SwordMovementExtensions
{
    public static void MoveSword(this ISwordMovement self, Vector3 lookDirection, Vector3? anchorPoint = null, Vector3? upDirection = null) => self.MoveSword(new ISwordMovement.MovementCommand { LookDirection = lookDirection, AnchorPoint = anchorPoint, UpDirection = upDirection });
}
