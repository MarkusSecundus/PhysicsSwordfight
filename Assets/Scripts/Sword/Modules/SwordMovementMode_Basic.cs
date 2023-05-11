using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.Input.Rays;
using MarkusSecundus.PhysicsSwordfight.Submodules;
using MarkusSecundus.PhysicsSwordfight.Utils.Graphics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Modules
{
    /// <summary>
    /// Basic sword control mode for slashing.
    /// 
    /// <para>
    /// Gets control point by intersecting <see cref="ISwordInput.GetInputRay"/> obtained from its parent <see cref="IScriptSubmodule{TScript}.Script"/> by an <see cref="IRayIntersectable"/>.
    /// <see cref="RayIntersection.InputorCenter"/> will be used as position of sword's anchor and <see cref="RayIntersection.Value"/> gives out the direction in which the sword is to be pointed.
    /// </para>
    /// </summary>
    [System.Serializable]
    public class SwordMovementMode_Basic : SwordMovement.Module
    {
        /// <summary>
        /// Geometric shape with which <see cref="ISwordInput.GetInputRay"/> gets intersected to obtain control point.
        /// </summary>
        [Tooltip("Geometric shape with which ISwordInput.GetInputRay() gets intersected to obtain control point")]
        public IRayIntersectable InputIntersectable;
        /// <summary>
        /// Sets sword rotation according to user input.
        /// </summary>
        /// <param name="delta">Time elapsed from last <see cref="OnFixedUpdate(float)"/></param>
        public override void OnFixedUpdate(float delta)
        {
            base.OnFixedUpdate(delta);

            SetSwordRotation(delta);
        }



        Vector3 computeUpVector(Vector3 forward)
        {
            var ret = Vector3.Cross(lastForward, forward).normalized;
            if (ret.y < 0 /* (ret.y == 0 && (ret.z < 0 || (ret.z == 0 && ret.x < 0)))*/ )
                ret = -ret;
            return ret;
        }


        const float minLastVectorDiff = 0.3f;
        Vector3 lastForward = Vector3.zero;
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
#if false //visualize
                var swordRay = Script.Sword.SwordBladeAsRay();
                Debug.DrawRay(inputRay.Value.origin-inputRay.Value.direction/1.5f, inputRay.Value.direction, Color.red);
                Debug.DrawRay(swordRay.origin, swordRay.direction, Color.cyan);
                DrawHelpers.DrawWireSphere(hitPoint, 0.02f, (a, b) => Debug.DrawLine(a, b, Color.blue), circles:50);
#endif
            }
        }
    }
}