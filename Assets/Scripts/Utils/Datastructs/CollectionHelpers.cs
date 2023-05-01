using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MarkusSecundus.Utils.Datastructs
{
    /// <summary>
    /// Static class containing convenience extensions methods for standard collections
    /// </summary>
    public static class CollectionHelpers
    {
        /// <summary>
        /// Yield given number of results obtained from a given supplier
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="supplier">Supplier for the iteration elements</param>
        /// <param name="count">How many elements</param>
        /// <returns>Generator that yields elements obtained from <paramref name="supplier"/></returns>
        public static IEnumerable<T> Repeat<T>(this System.Func<T> supplier, int count)
        {
            while (--count >= 0)
                yield return supplier();
        }

        /// <summary>
        /// Get last element of a list
        /// </summary>
        /// <typeparam name="T">Type of list's elements</typeparam>
        /// <param name="self">List to be peeked</param>
        /// <returns>Last element of the list</returns>
        public static T Peek<T>(this IReadOnlyList<T> self) => self[self.Count - 1];
        /// <summary>
        /// Get last element of a list if it has any
        /// </summary>
        /// <typeparam name="T">Type of list's elements</typeparam>
        /// <param name="self">List to be peeked</param>
        /// <param name="ret">Last element of the list or <c>default</c></param>
        /// <returns><c>true</c> if the list is non-empty</returns>
        public static bool TryPeek<T>(this IReadOnlyList<T> self, out T ret)
        {
            if (self.IsNullOrEmpty())
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
        /// <summary>
        /// Generator that lazily iterates through given stream given number of times
        /// </summary>
        /// <typeparam name="T">Type of stream's elements</typeparam>
        /// <param name="self">Stream to be iterated multiple times</param>
        /// <param name="repeatCount">How many times to iterate through the stream</param>
        /// <returns>Generator that lazily iterates through given stream given number of times</returns>
        public static IEnumerable<T> RepeatList<T>(this IEnumerable<T> self, int repeatCount)
        {
            while (--repeatCount >= 0)
                foreach (var i in self) yield return i;
        }

        /// <summary>
        /// Concatenates all elements into a string
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="self">Stream of elements to concatenate into a string</param>
        /// <param name="separator">Separator to be inserted between element's string representations</param>
        /// <returns>String concatenation of all provided elements</returns>
        public static string MakeString<T>(this IEnumerable<T> self, string separator = ", ")
        {
            using var it = self.GetEnumerator();

            if (!it.MoveNext()) return "";
            var ret = new StringBuilder().Append(it.Current.ToString());
            while (it.MoveNext()) ret = ret.Append(separator).Append(it.Current.ToString());

            return ret.ToString();
        }

        /// <summary>
        /// If the collection is null or empty
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="self">Collection to be checked for emptiness</param>
        /// <returns><c>true</c> iff the collection is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> self) => self.IsNil() || self.Count <= 0;

        /// <summary>
        /// Gets smallest value in a stream, using provided selector for comparisons.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TComp">Type of the elements used for comparison</typeparam>
        /// <param name="self">Stream to be searched through</param>
        /// <param name="selector">Function for obtaining comparable representative for each element</param>
        /// <returns>Value whose representative was the smallest</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">If the provided stream is empty</exception>
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
    }
}
