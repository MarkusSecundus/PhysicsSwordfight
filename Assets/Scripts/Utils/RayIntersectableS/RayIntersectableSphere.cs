using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RayIntersectableSphere : IRayIntersectable
{
    public Transform Center;
    public float Radius;

    public Vector3? GetIntersection(Ray r)
    {
        var ret = r.IntersectSphere(new Sphere(Center.position, Radius));
        if (ret.First != null) return ret.First.Value;
        if (ret.Second != null)return ret.Second.Value;
        return null;
    }
}
