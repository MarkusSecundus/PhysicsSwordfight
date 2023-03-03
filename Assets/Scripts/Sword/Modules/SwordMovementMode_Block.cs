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
    public Transform SwordDirectionHint;


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

    public Vector3 fixedHandlePoint => Script.FixedSwordHandlePoint;
    private Transform handleBegin => Script.descriptor.SwordAnchor;
    private Transform bladeTip => Script.descriptor.SwordTip;
    private Transform bladeEdgeBlockPoint => Script.descriptor.SwordBlockPoint;
    public override void OnFixedUpdate(float delta)
    {
        Cursor.lockState = CursorLockMode.Confined;
        var inputSphere = new Sphere(fixedHandlePoint, SwordLength);
        var input = Script.GetUserInput(inputSphere);

        if (input.First != null)
        {
            if (RegisterOnlyInputOnSphere && input.HasNullElement()) input.First = new Sphere(fixedHandlePoint, SwordLength).ProjectPoint(input.First.Value);
            SetBlockPosition(input.First.Value);
        }
    }

    private void SetBlockPosition(Vector3 hitPoint)
    {

        Ray localBladeAxis = new Ray(handleBegin.localPosition, bladeTip.localPosition - handleBegin.localPosition);
        var localBlockPointProjection = localBladeAxis.GetRayPointWithLeastDistance(bladeEdgeBlockPoint.localPosition);
        //Debug.Log($"quat: {Quaternion.LookRotation(SwordTip.localPosition - SwordHandle.localPosition, SwordEdgeBlockPoint.localPosition - localBlockPointProjection).eulerAngles}");


        Ray bladeAxis = new Ray(handleBegin.position, bladeTip.position - handleBegin.position);
        var blockPointProjection = bladeAxis.GetRayPointWithLeastDistance(bladeEdgeBlockPoint.position);
        float blockPointDistance = blockPointProjection.Distance(bladeEdgeBlockPoint.position),
              bladeTipDistance   = blockPointProjection.Distance(bladeTip.position),
              handleDistance     = blockPointProjection.Distance(handleBegin.position)
            ;

        Plane tangentialPlane = new Sphere(fixedHandlePoint, SwordLength).GetTangentialPlane(hitPoint);
        Vector3 normal = (hitPoint - fixedHandlePoint).normalized; //normal pointing in the direction outwards, away from the centre of the sphere

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
        Gizmos.DrawSphere(fixedHandlePoint, 0.01f);

        DrawHelpers.DrawWireSphere(fixedHandlePoint, SwordLength, Gizmos.DrawLine);
    }

}
