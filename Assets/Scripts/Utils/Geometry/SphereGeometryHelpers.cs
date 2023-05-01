using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    /// <summary>
    /// Static class providing methods for performing geometric computations on spheres
    /// </summary>
    public static class SphereGeometryHelpers
    {
        /// <summary>
        /// Iterate points on a circle.
        /// </summary>
        /// <param name="count">Number of points to iterate</param>
        /// <param name="begin">Value of the first point that describes the circle</param>
        /// <param name="axis">Axis around which the <paramref name="begin"/> vector is rotated</param>
        /// <param name="includeBegin">If <paramref name="begin"/> should be yielded as first value</param>
        /// <returns>Generator going through points of a circle</returns>
        public static IEnumerable<Vector3> PointsOnCircle(int count = 1, Vector3 begin = default, Vector3 axis = default, bool includeBegin = true)
        {
            if (count < 1) throw new ArgumentException("Must be a positive number", nameof(count));
            Vector3 v = begin == default ? new Vector3(1, 0, 0) : begin;

            if (includeBegin)
                yield return v;

            var rot = Matrix4x4.Rotate(Quaternion.AngleAxis(NumericConstants.MaxDegree / count, axis == default ? Vector3.forward : axis));
            for (int t = 1; t < count; ++t)
                yield return v = rot * v;
        }

        /// <summary>
        /// Get point on a unit circle corresponding to given angle in radians.
        /// </summary>
        /// <param name="angle_radians">Angle of the point in radians</param>
        /// <returns>Point on a unit circle corresponding to given angle</returns>
        public static Vector2 GetPointOnCircle(float angle_radians)
        {
            return new Vector2(Mathf.Cos(angle_radians), Mathf.Sin(angle_radians));
        }




        static (double? t1, double? t2) IntersectSphere_GetParameter(this ScaledRay self, Sphere sphere)
        {
            var (radius, centre) = (sphere.Radius, sphere.Center);

            if (self.direction == Vector3.zero)
                throw new ArgumentException(nameof(self), $"Direction must not be zero!");
            if (radius == 0)
                return (null, null);

            Vector3 o = self.origin - centre, d = self.direction;
            double a = (double)d.x * d.x + (double)d.y * d.y + (double)d.z * d.z; //non-zero if the direction is non-zero
            double b = 2 * ((double)o.x * d.x + (double)o.y * d.y + (double)o.z * d.z);
            double c = (double)o.x * o.x + (double)o.y * o.y + (double)o.z * o.z - radius * radius;

            return GeometryHelpers.SolveQuadraticEquation(a, b, c);
        }

        /// <summary>
        /// Compute intersection of a spherical surface with a ray.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public static (Vector3? First, Vector3? Second) IntersectSphere(this ScaledRay self, Sphere sphere)
            => self.GetPointsFromParameters(self.IntersectSphere_GetParameter(sphere));


        /// <summary>
        /// Project given point on a spherical surface.
        /// </summary>
        /// <param name="sphere">Spherical surface to project on</param>
        /// <param name="point">Point to project</param>
        /// <returns>Projection of <paramref name="point"/> on the surface of <paramref name="sphere"/></returns>
        public static Vector3 ProjectPoint(this Sphere sphere, Vector3 point)
        {
            var direction = (point - sphere.Center).normalized;
            return sphere.Center + direction * sphere.Radius;
        }

    }
}
