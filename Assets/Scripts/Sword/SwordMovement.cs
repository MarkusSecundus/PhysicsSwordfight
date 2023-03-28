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
    public SwordDescriptor descriptor;


    public ConfigurableJoint joint;
    private JointRotationHelper jointRotationHelper;

    public float HandleMovementInterpolationFactor = 9f;

    public SwordsmanDescriptor swordsmanDescriptor;
    private Transform swordAnchor => descriptor.SwordCenterOfMass;

    public Transform debuggerPoint;

    public float inputCircleRadius = 0.3f;

    public ISwordInput Input;

    public Vector3 FixedSwordHandlePoint => joint.connectedBody.transform.LocalToGlobal(originalConnectedAnchor);
    public Vector3 FlexSwordHandlePoint => swordAnchor.position;
    public float SwordLength => inputCircleRadius;
    public Rigidbody swordsmanBody => joint.connectedBody;

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
        
        UpdateAnchorPosition(delta);
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


    /// <summary>
    /// ..takes all global coords
    /// </summary>
    /// <param name="lookAt">what the blade should point at</param>
    /// <param name="anchor">where the swords anchor should move - remains unchanged if null</param>
    /// <param name="up">upvector for blade rotation, interpolated from this and last movement if null</param>
    public void SetSwordPosition(Vector3 lookAt, Vector3? anchor=null, Vector3? up=null)
    {
        //TODO: implement
    }



    public void SetSwordRotation(Quaternion rotation) 
    {
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
        intersection.First ??= UserInputFallback(ray, inputSphere);
        intersection.First = ClampInput(intersection.First, inputSphere);

        return intersection;
    }


    private Vector3 UserInputFallback(Ray inputRay, Sphere inputSphere)
    {
        var pointWithLeastDistance = inputRay.GetRayPointWithLeastDistance(inputSphere.Center);

        return pointWithLeastDistance;
    }

    private Vector3? ClampInput(Vector3? v, Sphere inputSphere)
    {
        if (!(v is Vector3 input)) return null;

        var clampingPlane = new Plane(swordsmanBody.transform.forward, swordsmanDescriptor.InputClampingBoundary.position);

        if (clampingPlane.SameSide(input, inputSphere.Center)) 
            return input;
        else
            return clampingPlane.ClosestPointOnPlane(input);
    }











    private void InitAnchorSetter()
    {
        joint.autoConfigureConnectedAnchor = false;
        this.targetConnectedAnchor=this.originalConnectedAnchor = joint.connectedAnchor;
    }

    private Vector3 originalConnectedAnchor;
    private Vector3 targetConnectedAnchor;
    public void MoveAnchorPosition(Vector3 absolutePosition)
    {
        var relative = joint.connectedBody.transform.GlobalToLocal(absolutePosition);
        targetConnectedAnchor = relative;
    }

    void UpdateAnchorPosition(float delta)
    {
        joint.connectedAnchor += (targetConnectedAnchor - joint.connectedAnchor) * delta * HandleMovementInterpolationFactor;
    }
}
