using MarkusSecundus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Interpolation
{
    /// <summary>
    /// Static class for utility functionalities concerning <see cref="RetargetableInterpolator{TValue, TInterpolationPolicy}"/>
    /// </summary>
    public static class RetargetableInterpolator
    {
        /// <summary>
        /// Config for <see cref="RetargetableInterpolator{TValue, TInterpolationPolicy}"/>
        /// </summary>
        [System.Serializable]
        public struct Config
        {
            /// <summary>
            /// Speed of the interpolation
            /// </summary>
            [SerializeField] public float InterpolationFactor;
        }

        /// <summary>
        /// Interpolation policy for lerping between <see cref="Vector3"/> values
        /// </summary>
        public struct VectorInterpolationPolicy : IFunc<Vector3, Vector3, float, Vector3>
        {
            /// <inheritdoc/>
            public Vector3 Invoke(Vector3 a, Vector3 b, float c) => Vector3.Lerp(a, b, c);
        }
        /// <summary>
        /// Interpolation policy for lerping between <see cref="System.Single"/> values
        /// </summary>
        public struct FloatInterpolationPolicy : IFunc<float, float, float, float>
        {
            /// <inheritdoc/>
            public float Invoke(float a, float b, float c) => Mathf.Lerp(a, b, c);
        }
    }

    /// <summary>
    /// Coroutine responsible for moving an object in fluent manner to movable target position
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TInterpolationPolicy"></typeparam>
    public class RetargetableInterpolator<TValue, TInterpolationPolicy> : IEnumerator where TInterpolationPolicy : struct, IFunc<TValue, TValue, float, TValue>
    {
        /// <summary>
        /// Getter for the value that's being interpolated
        /// </summary>
        public System.Func<TValue> Getter { get; init; }
        /// <summary>
        /// Setter for the value that's being interpolated
        /// </summary>
        public System.Action<TValue> Setter { get; init; }

        /// <summary>
        /// Yield-value describing update interval
        /// </summary>
        public object ToYield { get; init; }
        /// <summary>
        /// Callback for obtaining time passed from last update
        /// </summary>
        public System.Func<float> DeltaTimeGetter { get; init; }

        /// <summary>
        /// Value that is being moved to
        /// </summary>
        public TValue Target { get; set; }
        /// <summary>
        /// Config for the interpolation
        /// </summary>
        public RetargetableInterpolator.Config Config { get; init; }

        /// <summary>
        /// Internal coroutine stuff
        /// </summary>
        public object Current => ToYield;

        /// <summary>
        /// Internal coroutine stuff
        /// </summary>
        public bool MoveNext()
        {
            Setter(default(TInterpolationPolicy).Invoke(Getter(), Target, Config.InterpolationFactor * DeltaTimeGetter()));
            return true;
        }

        /// <summary>
        /// Internal coroutine stuff
        /// </summary>
        public void Reset() { }
    }
}