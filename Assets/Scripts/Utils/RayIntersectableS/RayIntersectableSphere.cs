using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RayIntersectableSphere : IRayIntersectable
{
    public Transform Center;
    public float Radius;
    public bool DrawGizmo = false;

    public override Vector3? GetIntersection(Ray r)
    {
        var ret = r.IntersectSphere(new Sphere(Center.position, Radius));
        if (ret.First != null) return ret.First.Value;
        if (ret.Second != null)return ret.Second.Value;
        return null;
    }

    private void OnDrawGizmos()
    {
        if (!DrawGizmo) return;

        Gizmos.color = Color.red;
        DrawHelpers.DrawWireSphere(Center.position, Radius, Gizmos.DrawLine);
    }
}
