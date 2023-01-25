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
    public float HandleSpeed_metersPerSecond = 6f;

    public SwordMovementMode_Block(SwordMovement script) : base(script){}

    public override void OnStart()
    {
        if (SwordLength <= 0) SwordLength = Script.SwordLength;
    }


    public override void OnDeactivated()
    {
        Script.SetAnchorPosition(Script.FixedSwordHandlePoint, HandleSpeed_metersPerSecond);
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

        Ray localBladeAxis = new Ray(SwordHandle.localPosition, SwordTip.localPosition - SwordHandle.localPosition);
        var localBlockPointProjection = localBladeAxis.GetRayPointWithLeastDistance(SwordEdgeBlockPoint.localPosition);
        //Debug.Log($"quat: {Quaternion.LookRotation(SwordTip.localPosition - SwordHandle.localPosition, SwordEdgeBlockPoint.localPosition - localBlockPointProjection).eulerAngles}");


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

        var upVector = computeUpVector();
        var swordRotation = Quaternion.LookRotation(tipPosition - handlePosition, upVector);

        Script.SetAnchorPosition(handlePosition, HandleSpeed_metersPerSecond);
        Script.SetSwordRotation(swordRotation);


        Script.SetDebugPointPosition(hitPoint);

        Vector3 computeUpVector()
        {
            //TODO: make it work in general case (so that it takes the blockPoint into consideration instead of assuming it to be orthogonal to the sword in the exact right way)
            return normal.Cross(bladeDirection);
        }
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
