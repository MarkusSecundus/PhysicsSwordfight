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
    public Camera inputCamera;

    public ConfigurableJoint joint;
    private JointRotationHelper jointRotationHelper;

    public Transform swordTip, swordAnchor;

    public WeaponDebugger debugger;
    public Transform debuggerPoint;

    public float inputCircleRadius = 0.3f;


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

    private Camera cameraToUse => inputCamera ?? Camera.main ?? throw new NullReferenceException("No camera found");



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


    private Vector3 originalConnectedAnchor { get; set; }
    private TweenerCore<Vector3, Vector3, VectorOptions> tween;
    public void SetAnchorPosition(Vector3 absolutePosition, float speed_metersPerSecond)
    {
        var relative = joint.connectedBody.transform.GlobalToLocal(absolutePosition);

        KillAnchorTween();
        if (speed_metersPerSecond.IsNaN())
        {
            joint.connectedAnchor = relative;
        }
        else
        {

        }
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

    public (Vector3? First, Vector3? Second) GetUserInput(Vector3 center, float radius)
    {
        var ray = cameraToUse.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);

        var intersection = ray.IntersectSphere(center, radius); 
        intersection.First ??= ray.GetRayPointWithLeastDistance(center);

        return intersection;
    }
}
