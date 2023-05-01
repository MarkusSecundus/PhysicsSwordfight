using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    /// <summary>
    /// Static class for defining commonly used numeric constants
    /// </summary>
    public static class NumericConstants
    {
        /// <summary>
        /// Number of degrees that corresponds to whole circle
        /// </summary>
        public const float MaxDegree = 360;
        /// <summary>
        /// Number of radians that corresponds to whole circle
        /// </summary>
        public const float MaxRadians = Mathf.PI * 2f;

        /// <summary>
        /// <see cref="Vector3"/> with all fields set to <c>NaN</c>
        /// </summary>
        public static readonly Vector3 NaNVector3 = new Vector3(float.NaN, float.NaN, float.NaN);

        /// <summary>
        /// Animation curve that's rougly 0 in the range [0;1) and roughly 1 in the range [1; +Inf)
        /// </summary>
        public static AnimationCurve AnimationCurve01 => new AnimationCurve(
            new Keyframe { time = -0.1f, value = -0.1f, inTangent = 0f, outTangent = 0f, inWeight = 0, outWeight = 1f / 3f },
            new Keyframe { time = 0.99f, value = -0.1f, inTangent = 0f, outTangent = 0f, inWeight = 1f / 3f, outWeight = 1 },
            new Keyframe { time = 1f, value = 1.1f, inTangent = 45f, outTangent = 45f, inWeight = 1f / 3f, outWeight = 0 }
        );
    }
}
