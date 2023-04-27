using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Randomness
{

    public static class RandomUtils
    {
        public static bool NextBool(this System.Random self) => self.Next(0, 2) == 1;
        public static float Next(this System.Random self, Interval<float> i) => self.NextFloat(i.Min, i.Max);
        public static int Next(this System.Random self, Interval<int> i) => self.Next(i.Min, i.Max + 1);
        public static Vector2 Next(this System.Random self, Interval<Vector2> i) => self.NextVector2(i.Min, i.Max);
        public static Vector3 Next(this System.Random self, Interval<Vector3> i) => self.NextVector3(i.Min, i.Max);

        public static bool Next(this System.Random self, Interval<bool> i) => i.Min == i.Max ? i.Min : (self.Next() & 1) == 0;

        public static int NextBitmap(this System.Random self, Interval<int> i)
        {
            var changeable = i.Max & ~i.Min;
            var randomBitmap = self.Next(int.MinValue, int.MaxValue);
            var toChange = changeable & randomBitmap;
            return i.Min | toChange;
        }
        public static T NextBitmap<T>(this System.Random self, Interval<T> i) where T : System.Enum => (T)(object)self.NextBitmap(new Interval<int>((int)(object)i.Min, (int)(object)i.Max));

        public static T NextElement<T>(this System.Random self, IReadOnlyList<T> list) => list[self.Next(0, list.Count)];

        public static void Shuffle<T>(this System.Random self, System.Span<T> toShuffle)
        {
            for (int t = 0; t < toShuffle.Length; ++t)
            {
                for (int u = t + 1; u < toShuffle.Length; ++u)
                    if (self.NextBool())
                        (toShuffle[t], toShuffle[u]) = (toShuffle[u], toShuffle[t]);
            }
        }


        public static T RandomElement<T>(this IReadOnlyList<T> self)
            => self[Random.Range(0, self.Count)];

        public static float NextFloat(this System.Random self, float min, float max) => (float)(min + self.NextDouble() * (max - min));

        public static Vector3 NextVector3(this System.Random self, Vector3 min, Vector3 max)
            => new Vector3(self.NextFloat(min.x, max.x), self.NextFloat(min.y, max.y), self.NextFloat(min.z, max.z));
        public static Vector2 NextVector2(this System.Random self, Vector2 min, Vector2 max)
            => new Vector2(self.NextFloat(min.x, max.x), self.NextFloat(min.y, max.y));

        public static Vector2 NextVector2(this System.Random self, Rect area) => self.NextVector2(area.min, area.max);

        public static Color NextRGBA(this System.Random self, Color min, Color max)
            => new Color(self.NextFloat(min.r, max.r), self.NextFloat(min.g, max.g), self.NextFloat(min.b, max.b), self.NextFloat(min.a, max.a));
        public static Color NextHSVA(this System.Random self, Color min, Color max)
            => self.NextVector3(min.AsVectorHSV(), max.AsVectorHSV()).AsHSV().With(a: self.NextFloat(min.a, max.a));

    }
}
