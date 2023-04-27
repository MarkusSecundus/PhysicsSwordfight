using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    public static class GeometryHelpers
    {
        public static (double? x1, double? x2) SolveQuadraticEquation(double a, double b, double c)
        {
            double D = b * b - 4 * a * c;

            if (D < 0) return (null, null);


            if (D == 0)
            {
                double t = -b / (2 * a);
                return (t, null);
            }
            //D > 0
            double SqrtD = System.Math.Sqrt(D);
            double x1 = (-b + SqrtD) / (2 * a),
                   x2 = (-b - SqrtD) / (2 * a);
            return (x1, x2);
        }

        public static (Vector3? First, Vector3? Second) GetPointsFromParameters(this Ray self, (double? t1, double? t2) parameters)
        {
            (var t1, var t2) = parameters;

            return (
                t1 == null ? (Vector3?)null : self.GetPoint(t1.Value),
                t2 == null ? (Vector3?)null : self.GetPoint(t2.Value)
                );
        }


        /// <summary>
        /// Computes angles of a triangle (described by the lengths of its sides) using the cosine theorem
        /// </summary>
        /// <returns></returns>
        public static (float a, float b, float c) GetTriangleAngles_sss(float lengthA, float lengthB, float lengthC)
        {
            var cosA = getCos(lengthA, lengthB, lengthC);
            var cosB = getCos(lengthB, lengthC, lengthA);
            var cosC = getCos(lengthC, lengthA, lengthB);

            return (Mathf.Acos(cosA) * Mathf.Rad2Deg, Mathf.Acos(cosB) * Mathf.Rad2Deg, Mathf.Acos(cosC) * Mathf.Rad2Deg);

            float getCos(float a, float b, float c) => (b * b + c * c - a * a) / (2 * b * c);
        }


        /// <summary>
        /// According to cosine lemma
        /// </summary>
        /// <returns>Angle in radians</returns>
        public static double ComputeTriangleAngle_sss(double oppositeSide, double adjacent1, double adjacent2)
        {
            if (adjacent1 == 0) throw new ArgumentException("Adjacent sides cannot be zero!", nameof(adjacent1));
            if (adjacent2 == 0) throw new ArgumentException("Adjacent sides cannot be zero!", nameof(adjacent2));

            double cos = (adjacent1 * adjacent1 + adjacent2 * adjacent2 - oppositeSide * oppositeSide) / (2 * adjacent1 * adjacent2);


            return Math.Acos(cos);
        }



    }
}
