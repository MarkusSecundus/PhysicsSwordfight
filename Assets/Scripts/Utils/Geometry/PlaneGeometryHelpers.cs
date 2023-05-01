using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    /// <summary>
    /// Static class providing methods for performing geometric computations on planes
    /// </summary>
    public static class PlaneGeometryHelpers
    {
        static readonly Matrix4x4 rotate90x = Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));
        static readonly Matrix4x4 rotate90y = Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0));
        static readonly Matrix4x4 rotate90z = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));

        /// <summary>
        /// Get orthogonal base vectors describing given 3D plane.
        /// </summary>
        /// <param name="plane">Plane in 3D space</param>
        /// <returns>Orthogonal base of the plane</returns>
        public static (Vector3 X, Vector3 Y) GetBase(this Plane plane)
        {
            Vector3 v1;
            if ((v1 = new Vector3(plane.normal.y, -plane.normal.x, 0)) == Vector3.zero && (v1 = new Vector3(plane.normal.z, 0, -plane.normal.x)) == Vector3.zero)
                v1 = new Vector3(0, -plane.normal.z, plane.normal.y);

            v1 = v1.normalized;
#if false
            /*v1 = plane.ClosestPointOnPlane(Vector3.one) - plane.GetShift();
            v1 = plane.normal.Cross(v1).normalized;*/

            /*if (!(v1 = rotate90x * plane.normal).Dot(plane.normal).IsNegligible() && !(v1 = rotate90y * plane.normal).Dot(plane.normal).IsNegligible())
                v1 = rotate90z * plane.normal;
            if (!v1.Dot(plane.normal).IsNegligible()) v1 = Vector3.Cross(plane.normal, v1);// Debug.Log($"Cross: {v1.Dot(plane.normal)}");*/
            //v1 = rotate90x * plane.normal;
#endif
            return (v1, Vector3.Cross(plane.normal, v1).normalized);
        }

        /// <summary>
        /// Convert given 2D vector to 3D vector according to the given orthogonal base.
        /// </summary>
        /// <param name="coordsBase">Orthogonal base describing the plane space.</param>
        /// <param name="v">2D vector to be converted to 3D</param>
        /// <returns>Converted vector</returns>
        public static Vector3 GetBasedVector(this (Vector3 X, Vector3 Y) coordsBase, Vector2 v)
        {
            return v.x * coordsBase.X + v.y * coordsBase.Y;
        }

        /// <summary>
        /// Get plane that's tangential to provided sphere in its given point.
        /// </summary>
        /// <param name="sphere">Sphere to which result plane is tangential</param>
        /// <param name="point">Point of the sphere contained in result plane</param>
        /// <returns>Plane that contains <paramref name="point"/> and is tangential to <paramref name="sphere"/></returns>
        public static Plane GetTangentialPlane(this Sphere sphere, Vector3 point)
        {
            //if (!point.Distance(sphere.Center).IsCloseTo(sphere.Radius)) throw new ArgumentException($"The point doesn't lay on the sphere");
            return new Plane(point - sphere.Center, point);
        }
    }
}
