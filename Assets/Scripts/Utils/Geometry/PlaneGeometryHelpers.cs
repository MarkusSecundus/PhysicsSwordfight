using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    public static class PlaneGeometryHelpers
    {
        private static readonly Matrix4x4 rotate90x = Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));
        private static readonly Matrix4x4 rotate90y = Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0));
        private static readonly Matrix4x4 rotate90z = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
        public static (Vector3 X, Vector3 Y) GetBase(this Plane plane)
        {
            Vector3 v1;
            if ((v1 = new Vector3(plane.normal.y, -plane.normal.x, 0)) == Vector3.zero && (v1 = new Vector3(plane.normal.z, 0, -plane.normal.x)) == Vector3.zero)
                v1 = new Vector3(0, -plane.normal.z, plane.normal.y);

            v1 = v1.normalized;
            /*v1 = plane.ClosestPointOnPlane(Vector3.one) - plane.GetShift();
            v1 = plane.normal.Cross(v1).normalized;*/

            /*if (!(v1 = rotate90x * plane.normal).Dot(plane.normal).IsNegligible() && !(v1 = rotate90y * plane.normal).Dot(plane.normal).IsNegligible())
                v1 = rotate90z * plane.normal;
            if (!v1.Dot(plane.normal).IsNegligible()) v1 = Vector3.Cross(plane.normal, v1);// Debug.Log($"Cross: {v1.Dot(plane.normal)}");*/
            //v1 = rotate90x * plane.normal;
            return (v1, Vector3.Cross(plane.normal, v1).normalized);
        }

        public static Vector3 GetBasedVector(this (Vector3 X, Vector3 Y) coordsBase, Vector2 v)
        {
            return v.x * coordsBase.X + v.y * coordsBase.Y;
        }

        public static Vector3 GetShift(this Plane self)
            => self.ClosestPointOnPlane(Vector3.zero);//throw new NotImplementedException("Plane shift is yet to be implemented!");//self.normal * self.distance; //DOESN'T WORK!!!!


        public static Plane GetTangentialPlane(this Sphere sphere, Vector3 point)
        {
            //if (!point.Distance(sphere.Center).IsCloseTo(sphere.Radius)) throw new ArgumentException($"The point doesn't lay on the sphere");
            return new Plane(point - sphere.Center, point);
        }

        public static Vector3 ProjectPoint(this Sphere sphere, Vector3 point)
        {
            var direction = (point - sphere.Center).normalized;
            return sphere.Center + direction * sphere.Radius;
        }
    }
}
