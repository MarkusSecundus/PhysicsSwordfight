using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input.Rays
{

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


        protected override RayIntersection GetIntersection_impl(Ray r)
        {
            Vector3 sum = Vector3.zero, center = Vector3.zero;
            float weightSum = 0;
            foreach (var e in entries)
            {
                var i = e.Value.GetIntersection(r);
                if (i.IsValid)
                {
                    var weight = i.Weight * e.Weight;
                    sum += i.Value * weight;
                    center += i.InputorCenter * weight;
                    weightSum += weight;
                }
            }
            return weightSum > 0 ? new RayIntersection(sum / weightSum, center / weightSum) : RayIntersection.Null;
        }

    }
}
