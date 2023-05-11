using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Randomness
{

    /// <summary>
    /// Static class containing convenience extensions methods for <see cref="System.Random"/>
    /// </summary>
    public static class RandomHelpers
    {
        /// <summary>
        /// Generate random <see cref="System.Boolean"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <returns>Random value</returns>
        public static bool NextBool(this System.Random self) => self.Next(0, 2) == 1;
        /// <summary>
        /// Generate random <see cref="System.Single"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="i">Range of the results, inclusive</param>
        /// <returns>Random value</returns>
        public static float Next(this System.Random self, Interval<float> i) => self.NextFloat(i.Min, i.Max);
        /// <summary>
        /// Generate random <see cref="System.Int32"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="i">Range of the results, inclusive</param>
        /// <returns>Random value</returns>
        public static int Next(this System.Random self, Interval<int> i) => self.Next(i.Min, i.Max + 1);
        /// <summary>
        /// Generate random <see cref="Vector2"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="i">Range of the results, inclusive</param>
        /// <returns>Random value</returns>
        public static Vector2 Next(this System.Random self, Interval<Vector2> i) => self.NextVector2(i.Min, i.Max);
        /// <summary>
        /// Generate random <see cref="Vector3"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="i">Range of the results, inclusive</param>
        /// <returns>Random value</returns>
        public static Vector3 Next(this System.Random self, Interval<Vector3> i) => self.NextVector3(i.Min, i.Max);

        /// <summary>
        /// Generate random <see cref="System.Boolean"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="i">Range of the results, inclusive</param>
        /// <returns>Random value</returns>
        public static bool Next(this System.Random self, Interval<bool> i) => i.Min == i.Max ? i.Min : self.NextBool();

        /// <summary>
        /// Randomly shuffle contents of provided span.
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="self">Source of randomness</param>
        /// <param name="toShuffle">Span to shuffle</param>
        public static void Shuffle<T>(this System.Random self, System.Span<T> toShuffle)
        {
            for (int t = 0; t < toShuffle.Length; ++t)
            {
                for (int u = t + 1; u < toShuffle.Length; ++u)
                    if (self.NextBool())
                        (toShuffle[t], toShuffle[u]) = (toShuffle[u], toShuffle[t]);
            }
        }

        /// <summary>
        /// Select random element from the provided list
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="self">Source of randomness</param>
        /// <returns>Randomly selected element from the list</returns>
        public static T RandomElement<T>(this IReadOnlyList<T> self)=> self[Random.Range(0, self.Count)];

        /// <summary>
        /// Generate random <see cref="System.Single"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="min">Min value, inclusive</param>
        /// <param name="max">Max value, inclusive</param>
        /// <returns>Random value</returns>
        public static float NextFloat(this System.Random self, float min, float max) => (float)(min + self.NextDouble() * (max - min));

        /// <summary>
        /// Generate random <see cref="Vector3"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="min">Min value, inclusive</param>
        /// <param name="max">Max value, inclusive</param>
        /// <returns>Random value</returns>
        public static Vector3 NextVector3(this System.Random self, Vector3 min, Vector3 max)
            => new Vector3(self.NextFloat(min.x, max.x), self.NextFloat(min.y, max.y), self.NextFloat(min.z, max.z));
        /// <summary>
        /// Generate random <see cref="Vector2"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="min">Min value, inclusive</param>
        /// <param name="max">Max value, inclusive</param>
        /// <returns>Random value</returns>
        public static Vector2 NextVector2(this System.Random self, Vector2 min, Vector2 max)
            => new Vector2(self.NextFloat(min.x, max.x), self.NextFloat(min.y, max.y));

        /// <summary>
        /// Generate random <see cref="Vector2"/>
        /// </summary>
        /// <param name="self">Source of randomness</param>
        /// <param name="area">Range of the results, inclusive</param>
        /// <returns>Random value</returns>
        public static Vector2 NextVector2(this System.Random self, Rect area) => self.NextVector2(area.min, area.max);
    }
}
