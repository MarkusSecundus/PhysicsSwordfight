using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Polynomial
{
    public float x3, x2, x1, c;

    public float Evaluate(float x)
    {
        float ret = c, pow = 1f;
        ret += (pow *= x) * x1;
        ret += (pow *= x) * x2;
        ret += (pow *= x) * x3;

        return ret;
    }

    public override string ToString() => $"{x3}x^3+{x2}x^2+{x1}x+{c}";

    public static readonly Polynomial Identity = new Polynomial { x1 = 1f };
}

