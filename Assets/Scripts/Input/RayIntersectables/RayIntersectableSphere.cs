using MarkusSecundus.PhysicsSwordfight.Utils.Geometry;
using MarkusSecundus.PhysicsSwordfight.Utils.Graphics;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input.Rays
{
    /// <summary>
    /// <see cref="IRayIntersectable"/> implementation of 3D sphere.
    /// </summary>
    [System.Serializable]
    public class RayIntersectableSphere : IRayIntersectable
    {
        /// <summary>
        /// What to do when the sphere was not hit
        /// </summary>
        public enum OnMissedPolicy
        {
            /// <summary>
            /// Just report <see cref="RayIntersection.Null"/>. Default behavior
            /// </summary>
            DoNothing, 
            /// <summary>
            /// Return point on the ray with least distance from the sphere's center
            /// </summary>
            TakeLeastDistance, 
            /// <summary>
            /// Take point on the ray with least distance from the sphere's center and project it on the sphere.
            /// </summary>
            Project
        }
        /// <summary>
        /// Center of the sphere
        /// </summary>
        public Transform Center;
        /// <summary>
        /// Radius of the sphere
        /// </summary>
        public float Radius;
        /// <summary>
        /// What to do when the sphere was not hit
        /// </summary>
        public OnMissedPolicy OnMissed = OnMissedPolicy.DoNothing;

        /// <inheritdoc/>
        protected override RayIntersection GetIntersection_impl(Ray r) => new RayIntersection(ComputeIntersection(r), Center.position);
        private Vector3? ComputeIntersection(Ray r)
        {
            var result = r.IntersectSphere(new Sphere(Center.position, Radius));
            if (result.First != null) return result.First.Value;
            Vector3? ret = OnMissed switch
            {
                OnMissedPolicy.TakeLeastDistance => r.GetRayPointWithLeastDistance(Center.position),
                OnMissedPolicy.Project => new Sphere(Center.position, Radius).ProjectPoint(r.GetRayPointWithLeastDistance(Center.position)),
                _ => null
            };
            return ret;
        }

        protected override void OnDrawGizmos()
        {
            if (false && (Hole != null || OnMissed != OnMissedPolicy.DoNothing))
                base.OnDrawGizmos();
            else
            {
                if (!ShouldDrawGizmo) return;

                Gizmos.color = GizmoColor;
                DrawHelpers.DrawWireSphere(Center.position, Radius, Gizmos.DrawLine);
            }
        }
    }
}