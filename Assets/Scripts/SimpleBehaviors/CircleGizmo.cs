using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;

[ExecuteInEditMode]
public class CircleGizmo : MonoBehaviour
{
    public Color Color = Color.black;
    public int Segments = 8;


    private void OnDrawGizmos()
    {
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
        Gizmos.DrawLine(lastPoint, beginPoint);
    }

    public IEnumerable<Vector3> IteratePoints()
        => GeometryUtils.PointsOnCircle(Segments).Select(transform.LocalToGlobal);
    [ContextMenu("Click me!?")]
    public void Ddsa()
    {
        Debug.Log("Clicked!!!");
    }
}
