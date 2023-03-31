using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;

public abstract class IPointsSupplier : MonoBehaviour
{
    public abstract IEnumerable<Vector3> IteratePoints();
}


public class CircleGizmo : IPointsSupplier
{
    public Color Color = Color.black;
    public int Segments = 8, MaxSegmentsToIterate=-1;
    public bool ShouldDrawTheGizmo = true;

    private void OnDrawGizmos()
    {
        if (!ShouldDrawTheGizmo) return;

        Gizmos.color = Color;

        Vector3 lastPoint = default, beginPoint = default;
        using var it = IteratePoints().GetEnumerator();
        if (it.MoveNext())
            lastPoint = beginPoint = it.Current;
        
        while (it.MoveNext())
        {
            Gizmos.DrawLine(lastPoint, it.Current);
            lastPoint = it.Current;
        }
        if(!IsCutOff)
            Gizmos.DrawLine(lastPoint, beginPoint);
    }

    private bool IsCutOff => MaxSegmentsToIterate >= 0 && MaxSegmentsToIterate < Segments;

    public override IEnumerable<Vector3> IteratePoints()
    {
        var ret = GeometryUtils.PointsOnCircle(Segments).Select(transform.LocalToGlobal);
        if (IsCutOff) ret = ret.Take(MaxSegmentsToIterate);
        return ret;
    }
}
