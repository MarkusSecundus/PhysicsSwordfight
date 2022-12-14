using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Linq;

public class SwordMovement : MonoBehaviour
{
    public Camera inputCamera;

    public ConfigurableJoint joint;
    private JointRotationHelper jointRotationHelper;

    public Transform swordTip, swordAnchor;

    public WeaponDebugger debugger;
    public Transform debuggerPoint;

    public float inputCircleRadius = 0.3f;


    public Vector3 swordHandlePoint => swordAnchor.position;
    public float swordLength => inputCircleRadius;


    private IScriptSubmodule<SwordMovement> activeMode;
    public SwordMovementModesContainer Modes;
    public Dictionary<KeyCode, IScriptSubmodule<SwordMovement>> modes;
    public SwordMovement()
    {
        Modes = new SwordMovementModesContainer { Basic = new SwordMovementMode_Basic(this), Block = new SwordMovementMode_Block(this) };
        modes = Modes.MakeMap();
    }

    void Start()
    {
        joint ??= GetComponent<ConfigurableJoint>();
        jointRotationHelper = joint.MakeRotationHelper();

        swordAnchor.localPosition = joint.anchor;
        debugger.AdjustPosition(joint);

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
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(swordHandlePoint, 0.01f);

        DrawHelpers.DrawWireSphere(swordHandlePoint, swordLength, Gizmos.DrawLine);
    }






    private TweenerCore<Vector3, Vector3, VectorOptions> tween = null;
    public void SetAnchorPosition(Vector3 position, float speed_metersPerSecond) 
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

    public Vector3? GetUserInput(Vector3 center, float radius)
    {
        var ray = cameraToUse.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);

        var intersection = ray.IntersectSphere(center, radius);

        return intersection.First ??= ray.GetRayPointWithLeastDistance(center);
    }
}
