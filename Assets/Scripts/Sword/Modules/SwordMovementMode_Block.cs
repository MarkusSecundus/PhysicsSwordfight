using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SwordMovementMode_Block : SwordMovement.Submodule
{
    public Transform SwordDirectionHint;
    public IRayIntersectable InputIntersector;

    SwordDescriptor Sword => Script.Sword;
    private Transform bladeEdgeBlockPoint => Sword.SwordBlockPoint;
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

        var bladeAxis = Sword.SwordBladeAsRay();
        var swordLength = bladeAxis.length;
        var blockPointProjection = bladeAxis.AsRay().GetRayPointWithLeastDistance(bladeEdgeBlockPoint.position);
        float bladeTipDistance   = blockPointProjection.Distance(bladeAxis.end),
              handleDistance     = blockPointProjection.Distance(bladeAxis.origin)
            ;

        var center = userInput.InputorCenter;
        Plane tangentialPlane = new Sphere(center, swordLength).GetTangentialPlane(hitPoint);
        Vector3 normal = (hitPoint - center).normalized; //normal pointing in the direction outwards, away from the centre of the sphere

        var bestDirectionHint = getBestDirectionHint(hitPoint);
        var projectedDirectionHint = tangentialPlane.ClosestPointOnPlane(bestDirectionHint);

        var bladeDirection = (projectedDirectionHint - hitPoint).normalized;

        var tipPosition = hitPoint + bladeDirection * bladeTipDistance;
        var handlePosition = hitPoint + (-bladeDirection) * handleDistance;

        Script.MoveSword(tipPosition - handlePosition, anchorPoint: handlePosition, upDirection: computeUpVector());

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
