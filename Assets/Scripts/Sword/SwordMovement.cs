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

    public PhysicsDrivenFollowPointBase.Configuration HandleMovementConfig = new PhysicsDrivenFollowPointBase.Configuration();

    public SwordsmanDescriptor swordsmanDescriptor;
    private Transform swordAnchor => descriptor.SwordCenterOfMass;

    public WeaponDebugger debugger;
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
        debugger.AdjustPosition(joint);

        InitAnchorSetter();

        foreach (var mode in modes.Values) mode.OnStart();
    }

    private void OnDestroy()
    {
        DisposeAnchorSetter();
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
        this.originalConnectedAnchor = joint.connectedAnchor;
        connectedAnchorTween = GameObjectUtils.InstantiateUtilObject($"{gameObject.name}_AnchorMovementModel", typeof(Rigidbody)).AddComponent<SwordHandleMovementSimulator>().Init(this, HandleMovementConfig);
    }
    private void DisposeAnchorSetter()
    {
        Destroy(connectedAnchorTween);
    }


    private SwordHandleMovementSimulator connectedAnchorTween;
    private Vector3 originalConnectedAnchor;
    public void MoveAnchorPosition(Vector3 absolutePosition)
    {
        var relative = joint.connectedBody.transform.GlobalToLocal(absolutePosition);
        connectedAnchorTween.PositionToFollow = relative;
        //joint.connectedAnchor = relative;
    }


    private class SwordHandleMovementSimulator : PhysicsDrivenFollowPointBase
    {
        protected override Vector3 GetFollowPosition() => PositionToFollow;

        public SwordMovement Script;
        public Vector3 PositionToFollow;

        public SwordHandleMovementSimulator Init(SwordMovement script, PhysicsDrivenFollowPointBase.Configuration config)
        {
            (Script, Config) = (script, config);
            return this;
        }

        protected override void Start()
        {
            base.Start();
            rb.position = PositionToFollow = Script.originalConnectedAnchor;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!Script.IsNil())
            {
                Script.joint.connectedAnchor = rb.position;
            }
        }
    }
}
