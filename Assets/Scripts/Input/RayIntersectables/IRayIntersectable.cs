using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Graphics;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Input.Rays
{

    [System.Serializable]
    public struct RayIntersection
    {
        public RayIntersection(Vector3? value, Vector3? inputorCenter, float weight = 1) => (this.value, InputorCenter, Weight) = (value, inputorCenter ?? default, weight <= 0f ? 0f : weight);

        public Vector3 Value => value.Value;
        private Vector3? value;
        public Vector3 InputorCenter;
        public float Weight;
        public bool IsValid => value != null && Weight > Mathf.Epsilon;

        public static RayIntersection Null => default;
    }

    public abstract class IRayIntersectable : MonoBehaviour
    {
        public bool ShouldDrawGizmo = false;
        public int GizmoSegments = 24;
        public float GizmoOvershoot = 1f;

        public Transform CenterOverride = null;
        public RayIntersectableSphere Hole;

        public AnimationCurve HoleWeight = NumericConstants.AnimationCurve01;

        public RayIntersection GetIntersection(Ray r)
        {
            var ret = GetIntersection_impl(r);
            if (!ret.IsValid) return ret;

            if (CenterOverride.IsNotNil()) ret.InputorCenter = CenterOverride.position;
            AdjustWeightsAccordingToHole(ref ret);

            return ret;
        }
        protected abstract RayIntersection GetIntersection_impl(Ray r);


        private void AdjustWeightsAccordingToHole(ref RayIntersection i)
        {
            if (Hole.IsNil()) return;
            var distance = Hole.Center.position.Distance(i.Value);
            var distanceRatio = distance / Hole.Radius;
            var weightAdjust = Mathf.Clamp(HoleWeight.Evaluate(distanceRatio), 0f, 1f);

            i.Weight *= weightAdjust;
        }

        protected virtual void OnDrawGizmos()
        {
            if (!ShouldDrawGizmo) return;
            Gizmos.color = Color.cyan;
            this.Visualize(Camera.main, Gizmos.DrawLine, GizmoSegments, GizmoOvershoot);
        }
    }

    public static class RayIntersectableExtensions
    {
        public static RayIntersection GetIntersection(this IRayIntersectable self, Ray? r)
        {
            if (r == null) return RayIntersection.Null;
            var ret = self.GetIntersection(r.Value);
            if (!ret.IsValid) return RayIntersection.Null;
            return ret;
        }

        public static void Visualize(this IRayIntersectable self, Camera camera, DrawHelpers.LineDrawer<Vector3> drawLine, int segments = 24, float overshoot = 1f)
        {
            var viewport = new Vector2(camera.pixelWidth, camera.pixelHeight);
            var plane = new Plane(new Vector3(0, 0, 1), Vector3.zero);
            DrawHelpers.DrawPlaneSegmentInterstepped(plane, viewport / 2, LineDrawer, diameter: viewport * overshoot, segments: segments);

            void LineDrawer(Vector3 a, Vector3 b)
            {
                var (v, w) = (transform(a), transform(b));
                if (v.IsValid && w.IsValid) drawLine(v.Value, w.Value);

                RayIntersection transform(Vector3 v)
                {
                    var ray = camera.ScreenPointToRay(v);
                    return self.GetIntersection(ray);
                }
            }
        }
    }
}