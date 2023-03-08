using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwordMovementMode_Basic : IScriptSubmodule<SwordMovement>
{
    public IRayIntersectable InputIntersectable;
    public SwordMovementMode_Basic(SwordMovement script) : base(script){}

    public override void OnFixedUpdate(float delta)
    {
        base.OnFixedUpdate(delta);

        SetSwordRotation(delta);
    }



    private Vector3 computeUpVector(Vector3 forward)
    {
        var ret = Vector3.Cross(lastForward, forward).normalized;
        if (ret.y < 0 /* (ret.y == 0 && (ret.z < 0 || (ret.z == 0 && ret.x < 0)))*/ )
            ret = -ret;
        return ret;
    }

    private Vector3 swordHandlePoint => Script.FlexSwordHandlePoint;
    private float swordLength => Script.SwordLength;


    public float minLastVectorDiff = 0.3f;
    private Vector3 lastForward = Vector3.zero;
    void SetSwordRotation(float delta)
    {
        //var inputSphere = new Sphere(swordHandlePoint, swordLength);
        //var input = Script.GetUserInput(inputSphere);
        var inputRay = Script.Input.GetInputRay();
        if (inputRay == null) return;
        var input = InputIntersectable.GetIntersection(inputRay.Value);

        if (input != null)
        {
            var hitPoint = input.Value;
            
#if false            
            {//debug
                var plane = new Sphere(swordHandlePoint, swordLength).GetTangentialPlane(hitPoint);
                DrawHelpers.DrawPlaneSegment(plane, hitPoint, (v, w) => Debug.DrawLine(v, w, Color.green));
            }
#endif
            var hitDirectionVector = (hitPoint - swordHandlePoint);

            Vector3 forward = hitDirectionVector, up = computeUpVector(forward);
            if (Vector3.Distance(lastForward, forward) >= minLastVectorDiff)
                lastForward = hitDirectionVector;

            var tr = Quaternion.LookRotation(hitDirectionVector, up);

            //Debug.DrawLine(swordHandlePoint, swordHandlePoint + up, Color.magenta);


            Script.SetSwordRotation(tr);
            Script.SetDebugPointPosition(hitPoint);
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        //Gizmos.DrawSphere(swordHandlePoint, 0.01f);

        //DrawHelpers.DrawWireSphere(swordHandlePoint, swordLength, Gizmos.DrawLine);
    }

}
