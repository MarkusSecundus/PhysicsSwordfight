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
        Input ??= GetComponent<ISwordInput>();

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
    [SerializeField]RetargetableInterpolator.Config HandleMovementInterpolationConfig = new RetargetableInterpolator.Config { InterpolationFactor = 9f };

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



    /// <summary>
    /// ..takes all global coords
    /// </summary>
    /// <param name="lookAt">what the blade should point at</param>
    /// <param name="anchor">where the swords anchor should move - remains unchanged if null</param>
    /// <param name="up">upvector for blade rotation, interpolated from this and last movement if null</param>
    public void SetSwordPosition(Vector3 lookAt, Vector3? anchor=null, Vector3? up=null)
    {
        Vector3 anchorPoint;
        if (anchor != null)
            MoveAnchorPosition(anchorPoint = anchor.Value);
        else anchorPoint = this.Joint.connectedBody.transform.LocalToGlobal(connectedAnchorPositioner.Target);


    }



    public void SetSwordRotation(Quaternion rotation) 
    {
        jointRotationHelper.SetTargetRotation(Quaternion.Inverse(Joint.connectedBody.transform.rotation) * rotation);
    }
    public void MoveAnchorPosition(Vector3 absolutePosition)
    {
        var relative = this.Joint.connectedBody.transform.GlobalToLocal(absolutePosition);
        connectedAnchorPositioner.Target = relative;
    }
    #endregion
}
