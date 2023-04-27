using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    public static class NumericConstants
    {
        public const float MaxDegree = 360;
        public const float MaxRadians = Mathf.PI * 2f;


        public static readonly Vector3 NaNVector3 = new Vector3(float.NaN, float.NaN, float.NaN);
    }
}
