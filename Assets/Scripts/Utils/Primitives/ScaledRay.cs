using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    [System.Serializable]
    public struct ScaledRay
    {
        public ScaledRay(Vector3 origin, Vector3 direction) => (this.origin, this.direction) = (origin, direction);

        public Vector3 origin, direction;

        public Vector3 end { get => origin + direction; set => direction = value - origin; }

        public float length => direction.magnitude;

        public static ScaledRay FromPoints(Vector3 origin, Vector3 end) => new ScaledRay(origin, end - origin);
    }

    public static class ScaledRayExtensions
    {
        public static Ray AsRay(this ScaledRay self) => new Ray(self.origin, self.direction);
        public static ScaledRay AsRay(this Ray self) => new ScaledRay(self.origin, self.direction);
    }


}
