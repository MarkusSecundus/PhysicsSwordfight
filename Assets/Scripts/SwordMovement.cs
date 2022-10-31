using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMovement : MonoBehaviour
{
    public float DamageWhenActive = 10f, DamageWhenIdle = 1f;

    public Camera inputCamera;

    public Vector3 mult = Vector3.one;
    public Vector3 targetRotation, debuggerRotation;
    public ConfigurableJoint joint;
    private JointRotationHelper jointRotationHelper;

    public ParaboloidSet controlParabola;

    public Transform swordTip, swordHandle;

    public Transform debugger, debuggerPoint;

    public float swordLengthModifier = 1f;

    public float inputCircleRadius = 0.3f;


    public Vector3 upVector = Vector3.up;

    public float minLastVectorDiff = 0.3f;


    public float idleDamper, maxDamper, minDamper;

    private Vector3 lastForward = Vector3.zero;

    private PhysicsDamager physicsDamager;


    private Vector3 lastBladetipPosition;

    // Start is called before the first frame update
    void Start()
    {
        physicsDamager = GetComponent<PhysicsDamager>();
        joint ??= GetComponent<ConfigurableJoint>();
        jointRotationHelper = joint.MakeRotationHelper(Space.Self);
        lastBladetipPosition = swordTip.transform.position;
    }

    private Camera cameraToUse => inputCamera ?? Camera.main ?? throw new NullReferenceException("No camera found");

    private float swordLength => Vector3.Distance(swordTip.position, swordHandle.position)*swordLengthModifier;

    private void rotateDebugger(Quaternion rotation)
    {
        debugger.transform.rotation = rotation;
        //Debug.Log($"{debugger.transform.name}| {rotation.eulerAngles} | glob: {debugger.transform.rotation.eulerAngles} | loc: {debugger.transform.localRotation.eulerAngles}");
    }


    private Vector3 computeUpVector(Vector3 forward)
    {
        var ret = Vector3.Cross(lastForward, forward).normalized;
        if (ret.y < 0 /*|| (ret.y == 0 && (ret.z < 0 || (ret.z == 0 && ret.x < 0)))*/ )
            ret = -ret;
        return ret;
    }



    // Update is called once per frame
    void Update()
    {

        SetSwordDamageBasedOnSpeed();

        Cursor.lockState = CursorLockMode.Confined;

        var ray = cameraToUse.ScreenPointToRay(Input.mousePosition);


        var swordHandlePoint = swordHandle.position;

        var intersection = ray.IntersectSphere(swordHandlePoint, swordLength);

        intersection.First ??= ray.GetRayPointWithLeastDistance(swordHandlePoint);

        if (intersection.First != null)
        {
            var hitPoint = intersection.First.Value;

            var hitDirectionVector = (hitPoint - swordHandlePoint);

            Vector3 forward = hitDirectionVector, up = computeUpVector(forward);
            if (Vector3.Distance(lastForward, forward) >= minLastVectorDiff)
                lastForward = hitDirectionVector;

            var tr = Quaternion.LookRotation(hitDirectionVector, up);
            //Debug.DrawLine(swordHandlePoint, swordHandlePoint + forward, Color.red);
            Debug.DrawLine(swordHandlePoint, swordHandlePoint + up, Color.magenta);


            rotateDebugger(tr);
            jointRotationHelper.SetTargetRotation(Quaternion.Inverse(joint.connectedBody.transform.rotation)*  tr);

            debuggerPoint.position = hitPoint;
            this.targetRotation = tr.eulerAngles;
        }


        this.debuggerRotation = this.transform.rotation.eulerAngles;
    }

    public float NonIdleTravelSpeed = 1f;

    public float travelSpeed_debug = -1f;

    void SetSwordDamageBasedOnSpeed()
    {
        var tipPosition = swordTip.transform.position;

        var pathTraveled = lastBladetipPosition - tipPosition;


        var travelSpeed = this.travelSpeed_debug = pathTraveled.magnitude / Time.deltaTime;

        this.physicsDamager.DamageMultiplier = travelSpeed >= NonIdleTravelSpeed ? DamageWhenActive : DamageWhenIdle;

        lastBladetipPosition = tipPosition;
    }
}
