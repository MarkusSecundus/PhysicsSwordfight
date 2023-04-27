using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MarkusSecundus.Utils.Datastructs
{
    public static class CollectionHelpers
    {
        public static IEnumerable<(T First, T Second)> AllCombinations<T>(this IReadOnlyList<T> l)
        {
            for (int t = 0; t < l.Count; ++t)
                for (int u = 0/*t+1*/; u < l.Count; ++u)
                    yield return (l[t], l[u]);
        }

        public static IEnumerable<T> Repeat<T>(this System.Func<T> supplier, int count)
        {
            while (--count >= 0)
                yield return supplier();
        }


        public static TResult Maximum<TResult, TCompare>(this IEnumerable<TResult> self, System.Func<TResult, TCompare> selector) where TCompare : System.IComparable<TCompare>
        {
            using var it = self.GetEnumerator();
            if (!it.MoveNext())
            {
                throw new System.IndexOutOfRangeException("Searching for max of an empty collection!");
            }
            var max = it.Current;
            var maxComparer = selector(max);

            while (it.MoveNext())
            {
                var toCompare = selector(it.Current);
                if (toCompare.CompareTo(maxComparer) > 0) (max, maxComparer) = (it.Current, toCompare);
            }
            return max;
        }

        public static T Peek<T>(this IReadOnlyList<T> self) => self[self.Count - 1];
        public static bool TryPeek<T>(this IReadOnlyList<T> self, out T ret)
        {
            if (self.IsEmpty())
            {
                ret = default;
                return false;
            }
            else
            {
                ret = self.Peek();
                return true;
            }
        }

        public static IEnumerable<T> RepeatList<T>(this IEnumerable<T> self, int repeatCount)
        {
            while (--repeatCount >= 0)
                foreach (var i in self) yield return i;
        }

        public static string MakeString<T>(this IEnumerable<T> self, string separator = ", ")
        {
            using var it = self.GetEnumerator();

            if (!it.MoveNext()) return "";
            var ret = new StringBuilder().Append(it.Current.ToString());
            while (it.MoveNext()) ret = ret.Append(separator).Append(it.Current.ToString());

            return ret.ToString();
        }


        public static bool IsEmpty<T>(this IReadOnlyCollection<T> self) => self == null || self.Count <= 0;
        public static Vector3 Average<T>(this IEnumerable<T> self, System.Func<T, Vector3> selector)
        {
            var (ret, count) = (Vector3.zero, 0);
            foreach (var i in self)
            {
                ret += selector(i);
                ++count;
            }
            return count <= 0 ? Vector3.zero : ret / count;
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> self, System.Func<T, bool> predicate, System.Func<T> defaultSupplier)
        {
            foreach (var i in self)
                if (predicate(i)) return i;

            return defaultSupplier();
        }
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self)
        {
            var ret = new Dictionary<TKey, TValue>();
            ret.AddAll(self);
            return ret;
        }
        public static ICollection<T> AddAll<T>(this ICollection<T> self, IEnumerable<T> toAdd)
        {
            foreach (var it in toAdd) self.Add(it);
            return self;
        }


        public static T Minimal<T, TComp>(this IEnumerable<T> self, System.Func<T, TComp> selector) where TComp : System.IComparable<TComp>
        {
            using var it = self.GetEnumerator();
            if (!it.MoveNext()) throw new System.ArgumentOutOfRangeException("Empty collection was provided!");

            var ret = it.Current;
            var min = selector(ret);

            while (it.MoveNext())
            {
                var cmp = selector(it.Current);
                if (cmp.CompareTo(min) < 0)
                {
                    min = cmp;
                    ret = it.Current;
                }
            }

            return ret;
        }


        public static bool IsNotNullNotEmpty<T>(this IReadOnlyList<T> self)
            => self != null && self.Count > 0;


    }
}
