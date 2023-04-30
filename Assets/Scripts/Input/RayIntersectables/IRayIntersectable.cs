using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Graphics;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using Newtonsoft.Json.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Input.Rays
{
    /// <summary>
    /// Struct describing the result of <see cref="IRayIntersectable.GetIntersection(Ray)"/>.
    /// </summary>
    [System.Serializable]
    public struct RayIntersection
    {
        /// <summary>
        /// Constructs the instance.
        /// </summary>
        /// <param name="value">Point of the intersection. Pass null to make this instance invalid.</param>
        /// <param name="inputorCenter">Center of the intersected object</param>
        /// <param name="weight">Weight of the result</param>
        public RayIntersection(Vector3? value, Vector3? inputorCenter, float weight = 1) => (this.value, InputorCenter, Weight) = (value, inputorCenter ?? default, weight <= 0f ? 0f : weight);

        /// <summary>
        /// Point of the intersection
        /// </summary>
        /// <exception cref="NullReferenceException">If <c>!<see cref="IsValid"/></c></exception>
        public Vector3 Value { get => value.Value; set => this.value = value; }
        private Vector3? value;
        /// <summary>
        /// Center of the intersected object.
        /// 
        /// The direction vector <c>(<see cref="Value"/> - <see cref="InputorCenter"/>)</c> is supposed to be used kinda as a normal to the point of intersection.
        /// </summary>
        public Vector3 InputorCenter;
        /// <summary>
        /// Weight of the result.
        /// 
        /// <para>Used mainly for interpolation purposes by <see cref="RayIntersectableInterpolation"/>.</para>
        /// </summary>
        public float Weight;
        /// <summary>
        /// Whether the intersection has a result.
        /// </summary>
        public bool IsValid => value != null && Weight > Mathf.Epsilon;
        
        /// <summary>
        /// Invalid <see cref="RayIntersection"/> with no result.
        /// </summary>
        public static RayIntersection Null => default;
    }

    /// <summary>
    /// Base for all objects that can provide an intersection with a ray.
    /// </summary>
    public abstract class IRayIntersectable : MonoBehaviour
    {
        /// <summary>
        /// Whether the gizmo vizualization should be drawn. Intended to be used from editor.
        /// </summary>
        public bool ShouldDrawGizmo = false;
        /// <summary>
        /// How detailed the gizmo vizualization should be. Affects performance. Intended to be used from editor.
        /// </summary>
        public int GizmoSegments = 24;
        /// <summary>
        /// How big is the radius in which the gizmo visualization rays should be thrown compared to the main camera's standard FOV. Intended to be used from editor.
        /// </summary>
        public float GizmoOvershoot = 1f;

        /// <summary>
        /// Doesn't affect computation of the intersection. If set, <c>position</c> of this will replace the original <see cref="RayIntersection.InputorCenter"/> in the return value of <see cref="GetIntersection(Ray)"/>.
        /// </summary>
        public Transform CenterOverride = null;

        /// <summary>
        /// Sphere that affects weights of the result intersections.
        /// </summary>
        public RayIntersectableSphere Hole;

        /// <summary>
        /// Curve that determines weights of the intersections, depending on <c>(<see cref="RayIntersection.Value"/>.Distance(Hole.Center)/Hole.Radius</c>
        /// </summary>
        public AnimationCurve HoleWeight = NumericConstants.AnimationCurve01;

        /// <summary>
        /// Try to intersect a ray with <c>this</c> object.
        /// </summary>
        /// <param name="r">Ray to try intersecting with <c>this</c></param>
        /// <returns>Result of the intersection</returns>
        public RayIntersection GetIntersection(Ray r)
        {
            var ret = GetIntersection_impl(r);
            if (!ret.IsValid) return ret;

            if (CenterOverride.IsNotNil()) ret.InputorCenter = CenterOverride.position;
            AdjustWeightsAccordingToHole(ref ret);

            return ret;
        }

        /// <summary>
        /// Computes the internal logic of the intersection.
        /// 
        /// <para>
        /// All the logic associated with <see cref="Hole"/> and <see cref="CenterOverride"/> is taken care of by the parent class.
        /// </para>
        /// </summary>
        /// <param name="r">Ray being intersected</param>
        /// <returns></returns>
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

    /// <summary>
    /// Static class containing convenient extension methods for <see cref="IRayIntersectable"/>.
    /// </summary>
    public static class RayIntersectableExtensions
    {

        /// <summary>
        /// Try to intersect a ray with <c>this</c> object.
        /// 
        /// Returns <see cref="RayIntersection.Null"/> if provided ray is <c>null</c>.
        /// </summary>
        /// <param name="self">Object to be intersected</param>
        /// <param name="r">Ray to try intersecting with <c>self</c></param>
        /// <returns>Result of the intersection</returns>
        public static RayIntersection GetIntersection(this IRayIntersectable self, Ray? r)
        {
            if (r == null) return RayIntersection.Null;
            var ret = self.GetIntersection(r.Value);
            if (!ret.IsValid) return RayIntersection.Null;
            return ret;
        }

        /// <summary>
        /// Visualizes the IRayIntersectable by throwing a lot of rays.
        /// </summary>
        /// <param name="self">Object to be visualized</param>
        /// <param name="camera">Camera from which to throw the rays</param>
        /// <param name="drawLine">Method to draw the lines</param>
        /// <param name="segments">How detailed the visualization should be</param>
        /// <param name="overshoot">How big is the radius in which the gizmo visualization rays should be thrown compared to the main camera's standard FOV</param>
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