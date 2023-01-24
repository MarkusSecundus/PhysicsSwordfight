using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Linq;

using Submodule = IScriptSubmodule<SwordMovement>;



public class SwordMovement : MonoBehaviour
{
    public ConfigurableJoint joint;
    private JointRotationHelper jointRotationHelper;

    public Transform swordTip, swordAnchor;

    public WeaponDebugger debugger;
    public Transform debuggerPoint;

    public float inputCircleRadius = 0.3f;

    public ISwordInput Input;

    public Vector3 FixedSwordHandlePoint => joint.connectedBody.transform.LocalToGlobal(originalConnectedAnchor);
    public Vector3 FlexSwordHandlePoint => swordAnchor.position;
    public float SwordLength => inputCircleRadius;


    private Submodule activeMode;
    public SwordMovementModesContainer Modes;
    public Dictionary<KeyCode, Submodule> modes;
    public SwordMovement()
    {
        Modes = new SwordMovementModesContainer(this);
    }

    void Start()
    {
        modes = Modes.MakeMap();
        joint ??= GetComponent<ConfigurableJoint>();
        jointRotationHelper = joint.MakeRotationHelper();

        swordAnchor.localPosition = joint.anchor;
        debugger.AdjustPosition(joint);

        InitAnchorSetter();

        foreach (var mode in modes.Values) mode.OnStart();
    }



    void Update()
    {
        MakeSureRightModeIsActive();
        activeMode?.OnUpdate(Time.deltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MakeSureRightModeIsActive();
        var delta = Time.fixedDeltaTime;

        activeMode?.OnFixedUpdate(delta);
        UpdateAnchorPosition(delta);
    }


    void MakeSureRightModeIsActive()
    {
        var mode = modes[modes.Keys.FirstOrDefault(Input.GetKey)];
        if (activeMode != mode)
        {
            activeMode?.OnDeactivated();
            (activeMode = mode)?.OnActivated();
        }
    }




    private void OnDrawGizmos()
    {
        activeMode?.OnDrawGizmos();
    }



    private void InitAnchorSetter()
    {
        joint.autoConfigureConnectedAnchor = false;
        this.originalConnectedAnchor = joint.connectedAnchor;
        KillAnchorTween();
    }

    private void KillAnchorTween()
    {
        tween?.Kill(); tween = null;
    }


    private TweenerCore<Vector3, Vector3, VectorOptions> tween;
    private Vector3 originalConnectedAnchor;
    //private Vector3 connectedAnchorTarget;
    //private float connectedAnchorChangeSpeedTarget;
    public void SetAnchorPosition(Vector3 absolutePosition, float speed_metersPerSecond)
    {
        var relative = joint.connectedBody.transform.GlobalToLocal(absolutePosition);
        if (true || !speed_metersPerSecond.IsNormalNumber() || relative.Distance(joint.connectedAnchor).IsNegligible(epsilon: 0.01f))
        {
            KillAnchorTween();
            joint.connectedAnchor = relative;
            return;
        }
        var travelTime = joint.connectedAnchor.Distance(relative) / speed_metersPerSecond;

        if(tween==null || tween.IsComplete())
        {
            tween = joint.DOConnectedAnchor(relative, travelTime);
        }
        else
        {
            //KillAnchorTween(); tween = joint.DOConnectedAnchor(relative, travelTime);
            tween.ChangeEndValue(relative, travelTime, true);
        }
        
        //connectedAnchorChangeSpeedTarget = speed_metersPerSecond;
        //connectedAnchorTarget = relative;


    }

    private void UpdateAnchorPosition(float delta)
    {

    }

    public void SetSwordRotation(Quaternion rotation) 
    {
        debugger?.RotateDebugger(rotation);
        jointRotationHelper.SetTargetRotation(Quaternion.Inverse(joint.connectedBody.transform.rotation) * rotation);
    }
    public void SetDebugPointPosition(Vector3 v)
    {
        if (debuggerPoint != null) debuggerPoint.position = v;

    }

    public (Vector3? First, Vector3? Second) GetUserInput(Sphere inputSphere)
    {
        if (!(Input.GetInputRay() is Ray ray)) return (null, null);
        
        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);

        var intersection = ray.IntersectSphere(inputSphere); 
        intersection.First ??= ray.GetRayPointWithLeastDistance(inputSphere.Center);

        return intersection;
    }
}
