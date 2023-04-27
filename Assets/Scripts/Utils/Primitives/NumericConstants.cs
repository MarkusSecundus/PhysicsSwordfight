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

        public static AnimationCurve AnimationCurve01 => new AnimationCurve(
            new Keyframe { time = -0.1f, value = -0.1f, inTangent = 0f, outTangent = 0f, inWeight = 0, outWeight = 1f / 3f },
            new Keyframe { time = 0.99f, value = -0.1f, inTangent = 0f, outTangent = 0f, inWeight = 1f / 3f, outWeight = 1 },
            new Keyframe { time = 1f, value = 1.1f, inTangent = 45f, outTangent = 45f, inWeight = 1f / 3f, outWeight = 0 }
        );
    }
}
