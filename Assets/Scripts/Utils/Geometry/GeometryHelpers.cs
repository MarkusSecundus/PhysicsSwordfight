using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    /// <summary>
    /// Static class providing methods that are generally useful in geometry computations
    /// </summary>
    public static class GeometryHelpers
    {
        /// <summary>
        /// Solves quadratic equation in format <c>a*x^2 + b*x + c = 0</c>. Doesn't consider imaginary solutions.
        /// </summary>
        /// <param name="a">Quadratic parameter</param>
        /// <param name="b">Linear parameter</param>
        /// <param name="c">Constant parameter</param>
        /// <returns>Both solutions if provided equation has two real solutions. Just <c>x1</c> if it has one, or <c>(null, null)</c> if the equation has no real solution.</returns>
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


        internal static (Vector3? First, Vector3? Second) GetPointsFromParameters(this ScaledRay self, (double? t1, double? t2) parameters)
        {
            (var t1, var t2) = parameters;

            return (
                t1 == null ? (Vector3?)null : self.GetPoint(t1.Value),
                t2 == null ? (Vector3?)null : self.GetPoint(t2.Value)
                );
        }
    }
}
