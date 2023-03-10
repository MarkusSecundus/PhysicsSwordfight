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
    public IRayIntersectable InputIntersector;

    public float SwordLength;
    public bool RegisterOnlyInputOnSphere = true;

    public SwordMovementMode_Block(SwordMovement script) : base(script){}

    public override void OnStart()
    {
        if (SwordLength <= 0) SwordLength = Script.SwordLength;
    }


    public override void OnDeactivated()
    {
        Script.MoveAnchorPosition(Script.FixedSwordHandlePoint);
    }

    public Vector3 fixedHandlePoint => Script.FixedSwordHandlePoint;
    private Transform handleBegin => Script.descriptor.SwordAnchor;
    private Transform bladeTip => Script.descriptor.SwordTip;
    private Transform bladeEdgeBlockPoint => Script.descriptor.SwordBlockPoint;
    public override void OnFixedUpdate(float delta)
    {
        var input = GetUserInput();
        if (input.IsValid) 
            SetBlockPosition(input);
    }

    private RayIntersection GetUserInput()
    {
        var ray = Script.Input.GetInputRay();
        if (ray == null) return RayIntersection.Null;
        return InputIntersector.GetIntersection(ray.Value);
    }

    private void SetBlockPosition(RayIntersection userInput)
    {
        var hitPoint = userInput.Value;

        Ray localBladeAxis = new Ray(handleBegin.localPosition, bladeTip.localPosition - handleBegin.localPosition);
        var localBlockPointProjection = localBladeAxis.GetRayPointWithLeastDistance(bladeEdgeBlockPoint.localPosition);
        //Debug.Log($"quat: {Quaternion.LookRotation(SwordTip.localPosition - SwordHandle.localPosition, SwordEdgeBlockPoint.localPosition - localBlockPointProjection).eulerAngles}");


        Ray bladeAxis = new Ray(handleBegin.position, bladeTip.position - handleBegin.position);
        var blockPointProjection = bladeAxis.GetRayPointWithLeastDistance(bladeEdgeBlockPoint.position);
        float blockPointDistance = blockPointProjection.Distance(bladeEdgeBlockPoint.position),
              bladeTipDistance   = blockPointProjection.Distance(bladeTip.position),
              handleDistance     = blockPointProjection.Distance(handleBegin.position)
            ;

        var center = userInput.InputorCenter;//fixedHandlePoint;
        Plane tangentialPlane = new Sphere(center, SwordLength).GetTangentialPlane(hitPoint);
        Vector3 normal = (hitPoint - center).normalized; //normal pointing in the direction outwards, away from the centre of the sphere

        var bestDirectionHint = getBestDirectionHint(hitPoint);
        var projectedDirectionHint = tangentialPlane.ClosestPointOnPlane(bestDirectionHint);

        var bladeDirection = (projectedDirectionHint - hitPoint).normalized;

        var tipPosition = hitPoint + bladeDirection * bladeTipDistance;
        var handlePosition = hitPoint + (-bladeDirection) * handleDistance;

        var upVector = computeUpVector();
        var swordRotation = Quaternion.LookRotation(tipPosition - handlePosition, upVector);

        Script.MoveAnchorPosition(handlePosition);
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


}
