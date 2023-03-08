using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class IRayIntersectable : MonoBehaviour
{
    public bool ShouldDrawGizmo = false;
    public int GizmoSegments = 24;
    public float GizmoOvershoot = 1f;
    public abstract Vector3? GetIntersection(Ray r);

    protected virtual void OnDrawGizmos()
    {
        if (!ShouldDrawGizmo) return;
        Gizmos.color = Color.cyan;
        this.Visualize(Camera.main, Gizmos.DrawLine, GizmoSegments, GizmoOvershoot);
    }
}

public static class RayIntersectableExtensions
{
    public static void Visualize(this IRayIntersectable self, Camera camera, DrawHelpers.LineDrawer<Vector3> drawLine, int segments=24, float overshoot = 1f)
    {
        var viewport = new Vector2(camera.pixelWidth, camera.pixelHeight);
        var plane = new Plane(new Vector3(0, 0, 1), Vector3.zero);
        DrawHelpers.DrawPlaneSegmentInterstepped(plane, viewport/2, LineDrawer, diameter: viewport*overshoot, segments:segments);

        void LineDrawer(Vector3 a, Vector3 b)
        {
            var (v, w) = (transform(a), transform(b));
            if (v != null && w != null) drawLine(v.Value, w.Value);

            Vector3? transform(Vector3 v)
            {
                var ray = camera.ScreenPointToRay(v);
                return self.GetIntersection(ray);
            }
        }
    }
}