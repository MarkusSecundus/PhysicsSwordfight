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


    public float minLastVectorDiff = 0.3f;
    private Vector3 lastForward = Vector3.zero;
    void SetSwordRotation(float delta)
    {
        var inputRay = Script.Input.GetInputRay();
        var input = InputIntersectable.GetIntersection(inputRay);

        if (input.IsValid)
        {
            var hitPoint = input.Value;
            var handlePoint = input.InputorCenter;
            var hitDirectionVector = (hitPoint - handlePoint);

            Vector3 forward = hitDirectionVector, up = computeUpVector(forward);
            if (Vector3.Distance(lastForward, forward) >= minLastVectorDiff)
                lastForward = hitDirectionVector;

            Script.MoveSword(hitDirectionVector, anchorPoint: handlePoint, upDirection: up);
        }
    }
}
