using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RayIntersectableSphere : IRayIntersectable
{
    public Transform Center;
    public float Radius;
    public float MaxAngle = 360f;

    public override Vector3? GetIntersection(Ray r)
    {
        var result = r.IntersectSphere(new Sphere(Center.position, Radius));
        if (IsValid(result.First)) return result.First.Value;
        if (IsValid(result.Second )) return result.Second.Value;
        return null;
    }
    private bool IsValid(Vector3? v)
    {
        if (v == null) return false;
        var angle = Vector3.Angle(transform.forward, v.Value - Center.position);
        if (Mathf.Abs(angle) > MaxAngle) return false;
        return true;
    }

    protected override void OnDrawGizmos()
    {
        if (MaxAngle < 360f)
            base.OnDrawGizmos();
        else
        {
            if (!ShouldDrawGizmo) return;

            Gizmos.color = Color.green;
            DrawHelpers.DrawWireSphere(Center.position, Radius, Gizmos.DrawLine);
        }
    }
}
