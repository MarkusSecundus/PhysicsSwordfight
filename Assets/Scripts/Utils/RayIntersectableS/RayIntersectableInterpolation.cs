using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RayIntersectableInterpolation : IRayIntersectable
{
    [System.Serializable]
    public struct Entry
    {
        public IRayIntersectable Value;
        public float Weight;
    }

    public Entry[] entries;


    public override Vector3? GetIntersection(Ray r)
    {
        Vector3 sum = Vector3.zero;
        float weightSum = 0;
        foreach(var e in entries)
        {
            if (e.Value.GetIntersection(r) is Vector3 i)
            {
                sum += i*e.Weight;
                weightSum += e.Weight;
            }
        }
        return weightSum > 0? (Vector3?)(sum/weightSum): null;
    }

}
