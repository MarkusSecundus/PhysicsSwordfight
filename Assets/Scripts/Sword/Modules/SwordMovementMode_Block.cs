using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MarkusSecundus.PhysicsSwordfight.Input.Rays;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.Utils.Datastructs;
using MarkusSecundus.PhysicsSwordfight.Utils.Geometry;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Modules
{

    /// <summary>
    /// Basic sword control mode for blocking enemy attacks.
    /// 
    /// <para>
    /// Gets control point by intersecting <see cref="ISwordInput.GetInputRay"/> obtained from its parent <see cref="IScriptSubmodule{TScript}.Script"/> by an <see cref="IRayIntersectable"/>.
    /// The sword will be positioned on a plane tangential to the intersected sphere in the point of intersection. It's blade pointing to direction of <see cref="SwordDirectionHint"/>.
    /// </para>
    /// </summary>
    [System.Serializable]
    public class SwordMovementMode_Block : SwordMovement.Module
    {
        /// <summary>
        /// Transform whose children transforms will be used as direction hints for the blocking mode.
        /// <para>
        /// The one child nearest to the control point will be chosed and used as the point to which the sword's blade should point to.
        /// </para>
        /// </summary>
        [Tooltip("Transform whose children transforms will be used as direction hints for the blocking mode")]
        public Transform SwordDirectionHint;
        /// <summary>
        /// Geometric shape with which <see cref="ISwordInput.GetInputRay"/> gets intersected to obtain control point.
        /// </summary>
        [Tooltip("Geometric shape with which ISwordInput.GetInputRay() gets intersected to obtain control point")]
        public IRayIntersectable InputIntersector;
        /// <summary>
        /// Value with which to multiply the default force with which the sword is held
        /// </summary>
        [Tooltip("Value with which to multiply the default force with which the sword is held")]
        public float HoldingForceMultiplier = 20f;


        SwordDescriptor Sword => Script.Sword;
        Transform bladeEdgeBlockPoint => Sword.SwordBlockPoint;


        /// <summary>
        /// Sets sword rotation according to user input.
        /// </summary>
        /// <param name="delta">Time elapsed from last <see cref="OnFixedUpdate(float)"/></param>
        public override void OnFixedUpdate(float delta)
        {
            var input = GetUserInput();
            if (input.IsValid)
                SetBlockPosition(input);
        }

        RayIntersection GetUserInput()
        {
            var ray = Script.Input.GetInputRay();
            if (ray == null) return RayIntersection.Null;
            return InputIntersector.GetIntersection(ray.Value);
        }

        void SetBlockPosition(RayIntersection userInput)
        {
            var hitPoint = userInput.Value;

            var bladeAxis = Sword.SwordBladeAsRay();
            var swordLength = bladeAxis.length;
            var blockPointProjection = bladeAxis.GetRayPointWithLeastDistance(bladeEdgeBlockPoint.position);
            float bladeTipDistance = blockPointProjection.Distance(bladeAxis.end),
                  handleDistance = blockPointProjection.Distance(bladeAxis.origin)
                ;

            var center = userInput.InputorCenter;
            Plane tangentialPlane = new Sphere(center, swordLength).GetTangentialPlane(hitPoint);
            Vector3 normal = (hitPoint - center).normalized; //normal pointing in the direction outwards, away from the centre of the sphere

            var bestDirectionHint = getBestDirectionHint(hitPoint);
            var projectedDirectionHint = tangentialPlane.ClosestPointOnPlane(bestDirectionHint);

            var bladeDirection = (projectedDirectionHint - hitPoint).normalized;

            var tipPosition = hitPoint + bladeDirection * bladeTipDistance;
            var handlePosition = hitPoint + (-bladeDirection) * handleDistance;

            Script.MoveSword(tipPosition - handlePosition, anchorPoint: handlePosition, upDirection: computeUpVector(), holdingForce: HoldingForceMultiplier);

            Vector3 computeUpVector()
            {
                //TODO: make it work in general case (so that it takes the blockPoint into consideration instead of assuming it to be orthogonal to the sword in the exact right way)
                return normal.Cross(bladeDirection);
            }
        }


        Vector3 getBestDirectionHint(Vector3 hitPoint)
        {
            return SwordDirectionHint.OfType<Transform>().Minimal(hint => hint.position.Distance(hitPoint)).position;
        }


    }
}