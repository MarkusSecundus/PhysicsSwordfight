using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    /// <summary>
    /// Simple object representing a 3D sphere, defined by centre and a radius
    /// </summary>
    [System.Serializable]
    public struct Sphere
    {
        /// <summary>
        /// Inits the sphere by given centre and radius
        /// </summary>
        /// <param name="center">Centre of the sphere</param>
        /// <param name="radius">Radius of the sphere</param>
        public Sphere(Vector3 center, float radius) => (Center, Radius) = (center, radius);

        /// <summary>
        /// Centre of the sphere
        /// </summary>
        public Vector3 Center;
        /// <summary>
        /// Radius of the sphere
        /// </summary>
        public float Radius;
    }

}
