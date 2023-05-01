using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    /// <summary>
    /// Object for defining intervals
    /// </summary>
    /// <typeparam name="T">Used numeric type</typeparam>
    [System.Serializable]
    public struct Interval<T>
    {
        /// <summary>
        /// Construct the interval
        /// </summary>
        /// <param name="min">Lower bound of the interval</param>
        /// <param name="max">Upper bound of the interval</param>
        public Interval(T min, T max) => (Min, Max) = (min, max);

        /// <summary>
        /// Lower bound of the interval
        /// </summary>
        public T Min;
        /// <summary>
        /// Upper bound of the interval
        /// </summary>
        public T Max;
    }

}
