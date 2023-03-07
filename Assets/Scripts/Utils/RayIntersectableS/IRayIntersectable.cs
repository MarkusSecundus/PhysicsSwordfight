using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class IRayIntersectable : MonoBehaviour
{
    public abstract Vector3? GetIntersection(Ray r);
}

public static class RayIntersectableExtensions
{
    public static void Visualize(this IRayIntersectable self, Camera camera, DrawHelpers.LineDrawer<Vector3> drawLine)
    {
        var plane = new Plane(new Vector3(0, 0, 1), Vector3.zero);
        DrawHelpers.DrawPlaneSegment(plane, Vector3.zero, LineDrawer);

        void LineDrawer(Vector3 a, Vector3 b)
        {
            var (v, w) = (transform(a), transform(b));
            if (v != null && w != null)
                drawLine(v.Value, w.Value);

            Vector3? transform(Vector3 v)
            {
                //var ray = new Ray(v.Value, plane.normal);
                var ray = camera.ScreenPointToRay(v);
                return self.GetIntersection(ray);
            }
        }
    }
}