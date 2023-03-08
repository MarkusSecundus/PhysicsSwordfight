using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RayIntersectableSphere : IRayIntersectable
{
    public Transform Center;
    public float Radius;
    public bool ProjectIfMissed = false;

    protected override RayIntersection GetIntersection_impl(Ray r) => new RayIntersection(ComputeIntersection(r), Center.position);
    private Vector3? ComputeIntersection(Ray r)
    {
        var result = r.IntersectSphere(new Sphere(Center.position, Radius));
        if (IsValid(result.First)) return result.First.Value;
        //if (IsValid(result.Second )) return result.Second.Value;
        if (ProjectIfMissed)
        {
            return new Sphere(Center.position, Radius).ProjectPoint(r.GetRayPointWithLeastDistance(Center.position));
        }
        return null;
    }
    private bool IsValid(Vector3? v)
    {
        if (v == null) return false;
        var direction = v.Value - Center.position;
        return true;
    }

    protected override void OnDrawGizmos()
    {
        if (Hole != null || ProjectIfMissed)
            base.OnDrawGizmos();
        else
        {
            if (!ShouldDrawGizmo) return;

            Gizmos.color = Color.green;
            DrawHelpers.DrawWireSphere(Center.position, Radius, Gizmos.DrawLine);
        }
    }
}
