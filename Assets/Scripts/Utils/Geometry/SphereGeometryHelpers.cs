using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    public static class SphereGeometryHelpers
    {
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

        public static Vector3 GetPointOnCircle(float angle_radians)
        {
            return new Vector3(Mathf.Cos(angle_radians), Mathf.Sin(angle_radians));
        }




        public static (double? t1, double? t2) IntersectSphere_GetParameter(this Ray self, Sphere sphere)
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

        public static (Vector3? First, Vector3? Second) IntersectSphere(this Ray self, Sphere sphere)
            => self.GetPointsFromParameters(self.IntersectSphere_GetParameter(sphere));

    }
}
