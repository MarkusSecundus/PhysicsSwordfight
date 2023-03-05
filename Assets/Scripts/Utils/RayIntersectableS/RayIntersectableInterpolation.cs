using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RayIntersectableInterpolation : IRayIntersectable
{
    public IRayIntersectable[] Spheres;


    public Vector3? GetIntersection(Ray r)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        bool someValueWasEncountered = false;
        foreach(var sphere in Spheres)
        {
            if (sphere.GetIntersection(r) is Vector3 i)
            {
                sum += i;
                someValueWasEncountered = true;
            }
            ++count;
        }

        return someValueWasEncountered? (Vector3?)sum/count : null;
    }
}
