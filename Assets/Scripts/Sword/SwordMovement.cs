using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class SwordMovement : MonoBehaviour
{
    public Camera inputCamera;

    public ConfigurableJoint joint;
    private JointRotationHelper jointRotationHelper;

    public Transform swordTip, swordAnchor;

    public WeaponDebugger debugger;
    public Transform debuggerPoint;

    public float inputCircleRadius = 0.3f;


    public Vector3 swordHandlePoint => swordAnchor.position;// joint.transform.position + joint.anchor;


    private IScriptSubmodule<SwordMovement> activeMode;
    public SwordMovementModes Modes;
    [System.Serializable]
    public struct SwordMovementModes
    {
        public SwordMovementMode_Basic Basic;
        public SwordMovementMode_Block Block;
    }
    public SwordMovement()
    {
        Modes = new SwordMovementModes { Basic = new SwordMovementMode_Basic(this) };
    }

    void Start()
    {
        joint ??= GetComponent<ConfigurableJoint>();
        jointRotationHelper = joint.MakeRotationHelper();

        swordAnchor.localPosition = joint.anchor;
        debugger.AdjustPosition(joint);

        activeMode = Modes.Basic;
        activeMode.OnActivated();
    }

    private Camera cameraToUse => inputCamera ?? Camera.main ?? throw new NullReferenceException("No camera found");

    public float swordLength => inputCircleRadius;


    void Update()
    {
        activeMode.OnUpdate(Time.deltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;

        activeMode.OnFixedUpdate(delta);
    }





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(swordHandlePoint, 0.01f);

        DrawHelpers.DrawWireSphere(swordHandlePoint, swordLength, Gizmos.DrawLine);
    }






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
