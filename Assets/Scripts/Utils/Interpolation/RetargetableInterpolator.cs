using MarkusSecundus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Interpolation
{

    public static class RetargetableInterpolator
    {
        [System.Serializable]
        public struct Config
        {
            [SerializeField] public float InterpolationFactor;
        }

        public struct VectorInterpolationPolicy : IFunc<Vector3, Vector3, float, Vector3>
        {
            public Vector3 Invoke(Vector3 a, Vector3 b, float c) => Vector3.Lerp(a, b, c);
        }
        public struct FloatInterpolationPolicy : IFunc<float, float, float, float>
        {
            public float Invoke(float a, float b, float c) => Mathf.Lerp(a, b, c);
        }
    }


    public class RetargetableInterpolator<TValue, TInterpolationPolicy> : IEnumerator where TInterpolationPolicy : struct, IFunc<TValue, TValue, float, TValue>
    {
        public System.Func<TValue> Getter { get; init; }
        public System.Action<TValue> Setter { get; init; }

        public object ToYield { get; init; }
        public System.Func<float> DeltaTimeGetter { get; init; }

        public TValue Target { get; set; }
        public RetargetableInterpolator.Config Config { get; init; }

        public object Current => ToYield;

        public bool MoveNext()
        {
            Setter(default(TInterpolationPolicy).Invoke(Getter(), Target, Config.InterpolationFactor * DeltaTimeGetter()));
            return true;
        }

        public void Reset() { }
    }
}