using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    /// <summary>
    /// Static class providing methods for performing geometric computations on rays
    /// </summary>
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
        /// <summary>
        /// Get shortes ray connecting two other rays in space.
        /// </summary>
        /// <param name="self">One ray</param>
        /// <param name="other">Other ray</param>
        /// <returns>Shortest connection between the two rays</returns>
        public static ScaledRay GetShortestRayConnection(this ScaledRay self, ScaledRay other)
        {
            var result = GetShortestRayConnection_impl(self, other);

            var resultOrigin = self.origin + (float)result.t1 * self.direction;
            var resultEnd = other.origin + (float)result.t2 * other.direction;
            return ScaledRay.FromPoints(resultOrigin, resultEnd);
        }

        /// <summary>
        /// Get shortes ray connecting two other rays in space. Rays are considered finite line segments beginning in <see cref="ScaledRay.origin"/> and ending in <see cref="ScaledRay.end"/>
        /// </summary>
        /// <param name="self">One ray</param>
        /// <param name="other">Other ray</param>
        /// <returns>Shortest connection between the two line segments</returns>
        public static ScaledRay GetShortestScaledRayConnection(this ScaledRay self, ScaledRay other)
        {
            var result = GetShortestRayConnection_impl(self, other);

            var resultOrigin = self.origin + Mathf.Clamp01((float)result.t1) * self.direction;
            var resultEnd = other.origin + Mathf.Clamp01((float)result.t2) * other.direction;
            return ScaledRay.FromPoints(resultOrigin, resultEnd);
        }


        static double GetRayPointWithLeastDistance_GetParameter(this ScaledRay self, Vector3 v)
        {
            return -Vector3.Dot((self.origin - v), self.direction) / self.direction.magnitude.Pow2();
        }

        /// <summary>
        /// Project given point on a given ray.
        /// </summary>
        /// <param name="self">Ray on which the projection lies.</param>
        /// <param name="v">Point to project</param>
        /// <returns>Projection of <paramref name="v"/> on <paramref name="self"/></returns>
        public static Vector3 GetRayPointWithLeastDistance(this ScaledRay self, Vector3 v)
            => self.GetPoint(self.GetRayPointWithLeastDistance_GetParameter(v));

        /// <summary>
        /// Get point going along the ray.
        /// </summary>
        /// <param name="self">Ray on which the point lies</param>
        /// <param name="t">Distance to travel from origin</param>
        /// <returns><c>self.origin + t*self.direction</c></returns>
        public static Vector3 GetPoint(this ScaledRay self, double t)
            => self.origin + (float)t * self.direction;
    }
}
