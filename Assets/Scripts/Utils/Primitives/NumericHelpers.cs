using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    public static class NumericHelpers
    {
        public static float Mod(this float f, float mod, out float div)
        {
            var d = System.Math.Floor(f / mod);
            div = (float)d;
            var ret = f - (d * mod);
            if (ret < 0f) ret += mod;
            return (float)ret;
        }
        public static float Mod(this float f, float mod) => f.Mod(mod, out _);


        public static double Pow2(this double d) => d * d;
        public static float Pow2(this float d) => d * d;

        public static float MinAbsolute(float a, float b) => (Mathf.Abs(a) <= Mathf.Abs(b)) ? a : b;
        public static float MaxAbsolute(float a, float b) => (Mathf.Abs(a) >= Mathf.Abs(b)) ? a : b;



        public static bool IsNaN(this float f) => float.IsNaN(f);
        public static bool IsPositiveInfinity(this float f) => float.IsPositiveInfinity(f);
        public static bool IsNegativeInfinity(this float f) => float.IsNegativeInfinity(f);

        public static bool IsNormalNumber(this float f) => !f.IsNaN() && !f.IsNegativeInfinity() && !f.IsPositiveInfinity();


        public static bool IsNegligible(this float f, float? epsilon = null) => Mathf.Abs(f) < (epsilon ?? Mathf.Epsilon);
        public static bool IsCloseTo(this float f, float g, float? epsilon = null) => (f - g).IsNegligible(epsilon);
    }
}
