using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RetargetableInterpolator
{
    [System.Serializable] public struct Config
    {
        [field: SerializeField] public float InterpolationFactor { get; init; }
    }

    public struct VectorInterpolationPolicy : IFunc<Vector3, Vector3, float, Vector3>
    {
        public Vector3 Invoke(Vector3 a, Vector3 b, float c) => Vector3.Lerp(a, b, c);
    }
}


public class RetargetableInterpolator<TValue, TInterpolationPolicy> : IEnumerator where TInterpolationPolicy: struct, IFunc<TValue, TValue, float, TValue>
{
    public System.Func<TValue> Getter { get; init; }
    public System.Action<TValue> Setter { get; init; }

    public object ToYield { get; init; }
    public System.Func<float> DeltaGetter { get; init; }

    public TValue Target { get; set; }
    public RetargetableInterpolator.Config Config { get; init; }

    public object Current => ToYield;

    public bool MoveNext()
    {
        Setter(default(TInterpolationPolicy).Invoke(Getter(), Target, Config.InterpolationFactor*DeltaGetter()));
        return true;
    }

    public void Reset(){}
}
