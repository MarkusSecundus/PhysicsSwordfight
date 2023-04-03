using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Linq;

using Submodule = IScriptSubmodule<SwordMovement>;
using Interpolator = RetargetableInterpolator<UnityEngine.Vector3, RetargetableInterpolator.VectorInterpolationPolicy>;


public class SwordMovement : MonoBehaviour
{
    public SwordDescriptor Sword;
    public ConfigurableJoint Joint;
    public ISwordInput Input;


    void Start()
    {
        Sword ??= GetComponent<SwordDescriptor>();
        Joint ??= GetComponent<ConfigurableJoint>();
        Input ??= GetComponentInParent<ISwordInput>();

        InitSwordMovers();
        InitModes();
    }

    #region Modes

    Submodule activeMode;
    public SwordMovementModesContainer Modes;
    Dictionary<KeyCode, Submodule> modes;
    public SwordMovement()
    {
        Modes = new SwordMovementModesContainer(this);
    }
    void InitModes()
    {
        modes = Modes.MakeMap();
        foreach (var mode in modes.Values) mode.OnStart();
    }

    void Update()
    {
        MakeSureRightModeIsActive();
        activeMode?.OnUpdate(Time.deltaTime);
    }
    void FixedUpdate()
    {
        MakeSureRightModeIsActive();
        activeMode?.OnFixedUpdate(Time.fixedDeltaTime);
    }
    void OnDrawGizmos() => activeMode?.OnDrawGizmos();

    void MakeSureRightModeIsActive()
    {
        var mode = modes[modes.Keys.FirstOrDefault(Input.GetKey)];
        if (activeMode != mode)
        {
            activeMode?.OnDeactivated();
            (activeMode = mode)?.OnActivated();
        }
    }
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


    [System.Serializable] public struct MovementCommand
    {
        [field: SerializeField]public Vector3 LookDirection { get; init; }
        [field: SerializeField]public Vector3? AnchorPoint { get; init; }
        [field: SerializeField]public Vector3? UpDirection { get; init; }
    }

    /// <summary>
    /// ..takes all global coords
    /// </summary>
    /// <param name="lookDirection">what the blade should point at</param>
    /// <param name="anchorPoint">where the swords anchor should move - remains unchanged if null</param>
    /// <param name="upDirection">upvector for blade rotation, interpolated from this and last movement if null</param>
    public void MoveSword(Vector3 lookDirection, Vector3? anchorPoint=null, Vector3? upDirection=null) => MoveSword(new MovementCommand { LookDirection = lookDirection, AnchorPoint = anchorPoint, UpDirection = upDirection });
    public void MoveSword(MovementCommand m)
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
