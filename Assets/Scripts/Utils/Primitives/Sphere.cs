using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    [System.Serializable]
    public struct Sphere
    {
        public Sphere(Vector3 center, float radius) => (Center, Radius) = (center, radius);

        public Vector3 Center;
        public float Radius;
    }

}
