using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public interface IRayIntersectable
{
    public Vector3? GetIntersection(Ray r);
}
