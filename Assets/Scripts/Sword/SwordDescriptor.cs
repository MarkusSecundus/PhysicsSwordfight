using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRayProvider
{
    public ScaledRay GetRay();
}

public class SwordDescriptor : MonoBehaviour, IRayProvider
{

    public Transform SwordAnchor => anchor = anchor.IfNil(target?.SwordAnchor);
    public Transform SwordCenterOfMass => centerOfWeight = centerOfWeight.IfNil(target?.SwordCenterOfMass);
    public Transform SwordTip => tipPoint = tipPoint.IfNil(target?.SwordTip);
    public Transform SwordHandleUpHandTarget => upHandTarget = upHandTarget.IfNil(target?.SwordHandleUpHandTarget);
    public Transform SwordHandleDownHandTarget => downHandTarget = downHandTarget.IfNil(target?.SwordHandleDownHandTarget);
    public Transform SwordBlockPoint => blockPoint = blockPoint.IfNil(target?.blockPoint);
    public Transform SwordBottom => bottom = bottom.IfNil(target?.bottom);
    public IReadOnlyList<Edge> Edges => edges = edges.IfNil(target?.edges);

    [SerializeField] private SwordDescriptor target = null;
    [SerializeField] private Transform anchor = null, centerOfWeight=null, tipPoint = null, blockPoint = null, upHandTarget = null, downHandTarget = null, bottom = null;
    [SerializeField] private Edge[] edges;

    [System.Serializable]
    public struct Edge
    {
        public Transform Root, Direction;
    }

    ScaledRay IRayProvider.GetRay() => this.SwordBladeAsRay();
}

public static class SwordDescriptorExtensions
{
    public static ScaledRay SwordBladeAsRay(this SwordDescriptor self)
    {
        var botom = self.SwordBottom.position;
        var direction = self.SwordTip.position - botom;
        return new ScaledRay(botom, direction);
    }
}
