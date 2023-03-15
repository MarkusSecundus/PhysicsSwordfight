using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UnorderedPair<T> : IEquatable<UnorderedPair<T>>
{
    public UnorderedPair(T a, T b) => (Left, Right) = (a, b);
    public UnorderedPair((T a, T b) t) :this(t.a, t.b) { }

    public T Left;
    public T Right;

    public override string ToString() => (Left, Right).ToString();
    public bool Equals(UnorderedPair<T> o) => object.Equals(Left, o.Left) && object.Equals(Right, o.Right) || (object.Equals(Left, o.Right) && object.Equals(Right, o.Left));
    public override bool Equals(object obj) => obj is UnorderedPair<T> t && Equals(t);

    public override int GetHashCode()
    {
        int a = Left.GetHashCode(), b = Right.GetHashCode();
        return a + b + a * b + 4899756;
    }
}
