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

    public bool OnlyIfClicked = true;


    public Vector3 upVector = Vector3.up;

    public float minLastVectorDiff = 0.3f;


    public float idleDamper, maxDamper, minDamper;

    private Vector3 lastForward = Vector3.zero;
    private Vector3 lastNormal = Vector3.zero;

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

    private Camera cameraToUse => inputCamera ?? Camera.main;

    private float swordLength => Vector3.Distance(swordTip.position, swordHandle.position)*swordLengthModifier;

    private Vector3 getAnchor() => joint.transform.TransformPoint(joint.anchor);


    private void rotateDebugger(Quaternion rotation)
    {
        debugger.transform.rotation = rotation;
        Debug.Log($"{debugger.transform.name}| {rotation.eulerAngles} | glob: {debugger.transform.rotation.eulerAngles} | loc: {debugger.transform.localRotation.eulerAngles}");
    }


    private Vector3 computeUpVector(Vector3 forward)
    {
        var ret = Vector3.Cross(lastForward, forward).normalized;
        if (ret.y < 0 /*|| (ret.y == 0 && (ret.z < 0 || (ret.z == 0 && ret.x < 0)))*/ )
            ret = -ret;
        return ret;
    }

    private float _damper
    {
        get => joint.slerpDrive.positionDamper;
        set
        {
            var d = joint.slerpDrive;
            if (d.positionDamper == value) return;
            d.positionDamper = value;
            joint.slerpDrive = d;
        }
    }

    private Func<float, float> damperInterpolationFunc = t => t * t;

    private float damperInterpolationProgress, damperInterpolationDuration = 1f, damperInterpolationBegin, damperInterpolationTarget;

    private void setDamper(float damperToSet, float duration)
    {
        if (damperToSet == _damper) return;
        damperInterpolationBegin = _damper;
        damperInterpolationTarget = damperToSet;
        damperInterpolationProgress = 0;
        damperInterpolationDuration = duration;
    }

    private void ProcessDamperInterpolation()
    {
        if (damperInterpolationProgress >= damperInterpolationDuration) return;
        damperInterpolationProgress += Time.deltaTime;
        float t = damperInterpolationProgress / damperInterpolationDuration;
        float diff = damperInterpolationTarget - damperInterpolationBegin;
        _damper = damperInterpolationBegin +  damperInterpolationFunc(t) * diff;
    }


    // Update is called once per frame
    void Update()
    {

        SetSwordDamageBasedOnSpeed();

        if (OnlyIfClicked && !Input.GetKey(KeyCode.Mouse0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            _damper = idleDamper;
            //setDamper(idleDamper, 0.3f);
        }
        else
        {
            _damper = minDamper;
            Cursor.lockState = CursorLockMode.Confined;

            var mousePos = Input.mousePosition;

            if (cameraToUse == null) return;
            var ray = cameraToUse.ScreenPointToRay(mousePos);
            /*ray = debugger.transform.parent.InverseTransformRay(ray);
            DebugDrawUtils.DrawRay(ray, Color.red);*/


            //Debug.Log($"Ray dst: {ray.PointDistanceFromRay(swordHandle.position)}");
            //Debug.Log($"Ray dst v2: {Vector3.Distance(swordHandle.position, ray.GetRayPointWithLeastDistance(swordHandle.position))}");


            var intersection = ray.IntersectSphere(swordHandle.position, swordLength);
                                                              //
            //Debug.Log($"Mouse: {mousePos}\nRay: {ray}");
            //Debug.Log($"Intersection:  {intersection}");

            if (intersection.First == null)
                intersection.First = ray.GetRayPointWithLeastDistance(swordHandle.position);

            if (intersection.First != null)
            {
                var swordHandlePoint = swordHandle.position;
                var hitPoint = intersection.First.Value;

                var hitDirectionVector = (hitPoint - swordHandlePoint);

                Vector3 forward = hitDirectionVector, up = computeUpVector(forward);
                if (Vector3.Distance(lastForward, forward) >= minLastVectorDiff)
                    lastForward = hitDirectionVector;
                lastNormal = up;

                var tr = Quaternion.LookRotation(hitDirectionVector, up);
                Debug.DrawLine(swordHandlePoint, swordHandlePoint + forward, Color.red);
                Debug.DrawLine(swordHandlePoint, swordHandlePoint + up, Color.magenta);


                rotateDebugger(tr);
                jointRotationHelper.SetTargetRotation(Quaternion.Inverse(debugger.transform.parent.rotation)*  tr);

                debuggerPoint.position = hitPoint;
                this.targetRotation = tr.eulerAngles;
            }
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
