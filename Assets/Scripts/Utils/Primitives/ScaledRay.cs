using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    /// <summary>
    /// Object describing 3D ray that remembers its length (unlike <see cref="Ray"/>).
    /// </summary>
    [System.Serializable]
    public struct ScaledRay
    {
        /// <summary>
        /// Begin point of the ray
        /// </summary>
        public Vector3 origin;
        /// <summary>
        /// Direction of the ray
        /// </summary>
        public Vector3 direction;

        /// <summary>
        /// End point of the ray
        /// </summary>
        public Vector3 end { get => origin + direction; set => direction = value - origin; }

        /// <summary>
        /// Length from <see cref="origin"/> to <see cref="end"/> of the ray
        /// </summary>
        public float length => direction.magnitude;

        /// <summary>
        /// Construct a scaled ray from origin and direction
        /// </summary>
        /// <param name="origin">Begin point of the ray</param>
        /// <param name="direction">Direction of the ray</param>
        /// <returns>Constructed ray</returns>
        public static ScaledRay FromDirection(Vector3 origin, Vector3 direction) => new ScaledRay {origin = origin, direction= direction };
        /// <summary>
        /// Construct a scaled ray from begin and end point
        /// </summary>
        /// <param name="origin">Begin point of the ray</param>
        /// <param name="end">End point of the ray</param>
        /// <returns>Constructed ray</returns>
        public static ScaledRay FromPoints(Vector3 origin, Vector3 end) => ScaledRay.FromDirection(origin, end - origin);
    }

    /// <summary>
    /// Static class containing convenience extensions methods for <see cref="ScaledRay"/>
    /// </summary>
    public static class ScaledRayExtensions
    {
        /// <summary>
        /// Convert <see cref="ScaledRay"/> to <see cref="Ray"/>
        /// </summary>
        /// <param name="self"></param>
        /// <returns><see cref="Ray"/> representation of <paramref name="self"/></returns>
        public static Ray AsRay(this ScaledRay self) => new Ray(self.origin, self.direction);
        /// <summary>
        /// Convert <see cref="Ray"/> to <see cref="ScaledRay"/>
        /// </summary>
        /// <param name="self"></param>
        /// <returns><see cref="ScaledRay"/> representation of <paramref name="self"/></returns>
        public static ScaledRay AsRay(this Ray self) => ScaledRay.FromDirection(self.origin, self.direction);
    }


}
