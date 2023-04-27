using MarkusSecundus.PhysicsSwordfight.Utils.Graphics;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input.Rays
{

    [System.Serializable]
    public class RayIntersectableSphere : IRayIntersectable
    {
        public enum OnMissedPolicy
        {
            DoNothing, TakeLeastDistance, Project
        }

        public Transform Center;
        public float Radius;
        public OnMissedPolicy OnMissed = OnMissedPolicy.DoNothing;

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
            //Debug.DrawRay(r.origin, r.direction, Color.yellow); 
            //if (ret != null) DrawHelpers.DrawWireSphere(ret.Value, 0.05f, (a, b) => Debug.DrawLine(a, b, Color.red));
            return ret;
        }

        protected override void OnDrawGizmos()
        {
            if (Hole != null || OnMissed != OnMissedPolicy.DoNothing)
                base.OnDrawGizmos();
            else
            {
                if (!ShouldDrawGizmo) return;

                Gizmos.color = Color.green;
                DrawHelpers.DrawWireSphere(Center.position, Radius, Gizmos.DrawLine);
            }
        }
    }
}