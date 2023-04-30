using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input.Rays
{

    /// <summary>
    /// Implementation if <see cref="IRayIntersectable"/> that interpolates between a list of other intersectable objects.
    /// </summary>
    [System.Serializable]
    public class RayIntersectableInterpolation : IRayIntersectable
    {
        /// <summary>
        /// Entry describing one segment of the interpolated object
        /// </summary>
        [System.Serializable]
        public struct Entry
        {
            /// <summary>
            /// The intersectable object
            /// </summary>
            public IRayIntersectable Value;
            /// <summary>
            /// Weight assigned to the intersectable object. Will be multiplied with <see cref="RayIntersection.Weight"/> to obtain definitive weight.
            /// </summary>
            public float Weight;
        }

        /// <summary>
        /// Segments of the interpolated object
        /// </summary>
        public Entry[] entries;

        /// <inheritdoc/>
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
