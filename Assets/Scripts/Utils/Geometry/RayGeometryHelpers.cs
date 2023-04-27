using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    public static class RayGeometryHelpers
    {
        private struct ShortestRayConnectionResult
        {
            public Vector3 resultDirection;
            public double t1, t2, t3;
        }

        private static ShortestRayConnectionResult GetShortestRayConnection_impl(ScaledRay self, ScaledRay other)
        {
            ///
            /// We are searching for a line that connects self and other in the shortest way possible. We know such line must be orthogonal to both self and other -> self.direction = self.direction.Cross(other.direction)
            /// Wow the only thing we need to find is result.origin.
            /// Lets assume we want result.origin to lie on self - then 
            ///		result.origin = self.origin + t1*self.direction for some t1
            /// Also result.origin + t3*result.direction = other.origin + t2*other.direction for some t3, t2 (the intersection of result with other)
            /// by substituting result.origin for the first line, we get:
            /// self.origin + t1*self.direction + t3*result.direction = other.origin + t2*other.direction
            /// By solving this system of linear equations (3 equations memberwise for xs, ys and zs) we obtain the 3 parameters that give us the result
            /// Default shape of the equation system is 
            ///		t1*self.direction + t2*(-other.direction) + t3*result.direction = -self.origin + other.origin
            /// 

            var resultDirection = self.direction.Cross(other.direction);

            Vector3 a = self.direction, b = -other.direction, c = resultDirection, d = -self.origin + other.origin;

            var equationParams = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(new double[,]
            {
            { a.x, b.x, c.x},
            { a.y, b.y, c.y},
            { a.z, b.z, c.z}
            });
            var constants = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] { d.x, d.y, d.z });

            var solution = equationParams.Solve(constants);

            double t1 = solution[0], t2 = solution[1], t3 = solution[2];

            return new ShortestRayConnectionResult { resultDirection = resultDirection, t1 = t1, t2 = t2, t3 = t3 };
        }
        public static ScaledRay GetShortestRayConnection(this ScaledRay self, ScaledRay other)
        {
            var result = GetShortestRayConnection_impl(self, other);

            var resultOrigin = self.origin + (float)result.t1 * self.direction;
            var resultEnd = other.origin + (float)result.t2 * other.direction;
            return ScaledRay.FromPoints(resultOrigin, resultEnd);
        }

        //Same as GetShortestRayConnection(..) but considers the ray to represent finite line segments (that have a beginning and an end)
        public static ScaledRay GetShortestScaledRayConnection(this ScaledRay self, ScaledRay other)
        {
            var result = GetShortestRayConnection_impl(self, other);

            var resultOrigin = self.origin + Mathf.Clamp01((float)result.t1) * self.direction;
            var resultEnd = other.origin + Mathf.Clamp01((float)result.t2) * other.direction;
            return ScaledRay.FromPoints(resultOrigin, resultEnd);
        }


        public static double GetRayPointWithLeastDistance_GetParameter(this Ray self, Vector3 v)
        {
            return -Vector3.Dot((self.origin - v), self.direction) / self.direction.magnitude.Pow2();
        }

        public static Vector3 GetRayPointWithLeastDistance(this Ray self, Vector3 v)
            => self.GetPoint(self.GetRayPointWithLeastDistance_GetParameter(v));


        public static Vector3 GetPoint(this Ray self, double t)
            => self.origin + (float)t * self.direction;

        public static double PointDistanceFromRay(this Ray self, Vector3 v)
        {
            Vector3 o = self.origin, s = self.direction;

            double dst = Vector3.Cross(s, o - v).sqrMagnitude / s.sqrMagnitude;

            return dst;
        }

    }
}
