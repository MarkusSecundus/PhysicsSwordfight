using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SwordMovementMode_Block : IScriptSubmodule<SwordMovement>
{
    public Transform SwordHandle, SwordTip, SwordEdgeBlockPoint, SwordDirectionHint;

    public float SwordLength;
    public bool RegisterOnlyInputOnSphere = true;

    public SwordMovementMode_Block(SwordMovement script) : base(script){}

    public override void OnStart()
    {
        if (SwordLength <= 0) SwordLength = Script.SwordLength;
    }

    public Vector3 swordHandlePoint => Script.FixedSwordHandlePoint;
    public override void OnFixedUpdate(float delta)
    {
        Cursor.lockState = CursorLockMode.Confined;
        var inputSphere = new Sphere(swordHandlePoint, SwordLength);
        var input = Script.GetUserInput(inputSphere);

        if (input.First != null)
        {
            if (RegisterOnlyInputOnSphere && input.HasNullElement()) input.First = new Sphere(swordHandlePoint, SwordLength).ProjectPoint(input.First.Value);
            SetBlockPosition(input.First.Value);
        }
    }

    private void SetBlockPosition(Vector3 hitPoint)
    {
        Ray bladeAxis = new Ray(SwordHandle.position, SwordTip.position - SwordHandle.position);
        var blockPointProjection = bladeAxis.GetRayPointWithLeastDistance(SwordEdgeBlockPoint.position);
        float blockPointDistance = blockPointProjection.Distance(SwordEdgeBlockPoint.position),
              bladeTipDistance   = blockPointProjection.Distance(SwordTip.position),
              handleDistance     = blockPointProjection.Distance(SwordHandle.position)
            ;

        Plane tangentialPlane = new Sphere(swordHandlePoint, SwordLength).GetTangentialPlane(hitPoint);
        Vector3 normal = (hitPoint - swordHandlePoint).normalized; //normal pointing in the direction outwards, away from the centre of the sphere

        var bestDirectionHint = getBestDirectionHint(hitPoint);
        var projectedDirectionHint = tangentialPlane.ClosestPointOnPlane(bestDirectionHint);

        var bladeDirection = (projectedDirectionHint - hitPoint).normalized;

        var tipPosition = hitPoint + bladeDirection * bladeTipDistance;
        var handlePosition = hitPoint + (-bladeDirection) * handleDistance;

        var swordRotation = Quaternion.LookRotation(tipPosition - handlePosition, normal);

        Script.SetAnchorPosition(handlePosition, float.NaN);
        Script.SetSwordRotation(swordRotation);


        Script.SetDebugPointPosition(hitPoint);
    }


    private Vector3 getBestDirectionHint(Vector3 hitPoint)
    {
        return SwordDirectionHint.OfType<Transform>().Minimal(hint => hint.position.Distance(hitPoint)).position;
    }


    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(swordHandlePoint, 0.01f);

        DrawHelpers.DrawWireSphere(swordHandlePoint, SwordLength, Gizmos.DrawLine);
    }

}
