using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class SwordMovement : MonoBehaviour
{
    public float DamageWhenActive = 10f, DamageWhenIdle = 1f;

    public Camera inputCamera;

    public Vector3 targetRotation, debuggerRotation;
    public ConfigurableJoint joint;
    private JointRotationHelper jointRotationHelper;

    public Transform swordTip, swordAnchor;

    public WeaponDebugger debugger;
    public Transform debuggerPoint;

    public float inputCircleRadius = 0.3f;


    public float minLastVectorDiff = 0.3f;

    public float anchorMoveModifier = 1f;


    private Vector3 lastForward = Vector3.zero;

    private PhysicsDamager physicsDamager;


    private Vector3 lastBladetipPosition;

    private Vector3 swordHandlePoint => swordAnchor.position;// joint.transform.position + joint.anchor;

    public BlockingConfig blockingConfig = new BlockingConfig();

    [System.Serializable]
    public class BlockingConfig
    {
        public Transform BlockingPosition;
        public float BlockBeginDuration = 0.5f;
        public float BlockEndDuration = 0.3f;
    }


    void Start()
    {
        physicsDamager = GetComponent<PhysicsDamager>();
        joint ??= GetComponent<ConfigurableJoint>();
        jointRotationHelper = joint.MakeRotationHelper();
        lastBladetipPosition = swordTip.transform.position;


        swordAnchor.localPosition = joint.anchor;
        debugger.AdjustPosition(joint);
        joint.autoConfigureConnectedAnchor = false;
        originalConnectedAnchor = joint.connectedAnchor;
    }

    private Camera cameraToUse => inputCamera ?? Camera.main ?? throw new NullReferenceException("No camera found");

    private float swordLength => inputCircleRadius;



    private Vector3 computeUpVector(Vector3 forward)
    {
        var ret = Vector3.Cross(lastForward, forward).normalized;
        if (ret.y < 0 /* (ret.y == 0 && (ret.z < 0 || (ret.z == 0 && ret.x < 0)))*/ )
            ret = -ret;
        return ret;
    }



    // Update is called once per frame
    void Update()
    {
        var delta = Time.fixedDeltaTime;
        Cursor.lockState = CursorLockMode.Confined;

        SetSwordDamageBasedOnSpeed(delta);
        SetSwordRotation(delta);
        ManageBlocking(delta);
    }



    private Vector3 originalConnectedAnchor;
    private TweenerCore<Vector3, Vector3, VectorOptions> tween = null;
    void ManageBlocking(float delta)
    {
        const KeyCode BlockKey = KeyCode.LeftShift;
        if (Input.GetKeyDown(BlockKey)) StartBlock();
        else if (Input.GetKeyUp(BlockKey)) EndBlock();

        void StartBlock()
        {
            if (tween != null) { tween.Kill(); tween = null; }
            var endValue = originalConnectedAnchor + blockingConfig.BlockingPosition.localPosition;
            tween = joint.DOConnectedAnchor(endValue, blockingConfig.BlockBeginDuration);
        }
        void EndBlock() 
        {
            tween?.Kill();
            tween = null;
            tween = joint.DOConnectedAnchor(originalConnectedAnchor, blockingConfig.BlockBeginDuration);
        }
    }


    void SetSwordRotation(float delta)
    {
        var input = GetUserInput(swordHandlePoint, swordLength);

        if (input != null)
        {
            var hitPoint = input.Value;

            {//debug
                var plane = (swordHandlePoint, swordLength).GetTangentialPlane(hitPoint);
                DrawHelpers.DrawPlaneSegment(plane, hitPoint, (v,w)=>Debug.DrawLine(v,w, Color.green));
            }

            var hitDirectionVector = (hitPoint - swordHandlePoint);

            Vector3 forward = hitDirectionVector, up = computeUpVector(forward);
            if (Vector3.Distance(lastForward, forward) >= minLastVectorDiff)
                lastForward = hitDirectionVector;

            var tr = Quaternion.LookRotation(hitDirectionVector, up);

            Debug.DrawLine(swordHandlePoint, swordHandlePoint + up, Color.magenta);


            SetSwordRotation(tr);
            SetDebugPointPosition(hitPoint);
        }
        this.debuggerRotation = this.transform.rotation.eulerAngles;
    }





    public float NonIdleTravelSpeed = 1f;
    //public float travelSpeed_debug = -1f;
    void SetSwordDamageBasedOnSpeed(float delta)
    {
        var tipPosition = swordTip.transform.position;

        var pathTraveled = lastBladetipPosition - tipPosition;


        var travelSpeed = /*this.travelSpeed_debug =*/ pathTraveled.magnitude / delta;

        this.physicsDamager.DamageMultiplier = travelSpeed >= NonIdleTravelSpeed ? DamageWhenActive : DamageWhenIdle;

        lastBladetipPosition = tipPosition;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(swordHandlePoint, 0.01f);

        DrawHelpers.DrawWireSphere(swordHandlePoint, swordLength, Gizmos.DrawLine);
    }




    public void SetAnchorPosition(Vector3 position, float speed_metersPerSecond) { }

    public void SetSwordRotation(Quaternion rotation) 
    {
        debugger?.RotateDebugger(rotation);
        jointRotationHelper.SetTargetRotation(Quaternion.Inverse(joint.connectedBody.transform.rotation) * rotation);

        this.targetRotation = rotation.eulerAngles;
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
