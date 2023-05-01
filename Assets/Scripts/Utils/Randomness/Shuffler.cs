using MarkusSecundus.Utils.Datastructs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Randomness
{
    /// <summary>
    /// Source of random elements operating via a shuffled list.
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    public struct Shuffler<T>
    {

        /// <summary>
        /// Init the shuffler
        /// </summary>
        /// <param name="randomizer">Source of randomness</param>
        /// <param name="items">List of items to be randomized</param>
        /// <param name="windowSize">Window size in multiples of <c>items.Count</c></param>
        public Shuffler(System.Random randomizer, IReadOnlyList<T> items, int windowSize)
        {
            rand =  randomizer;
            window = items.RepeatList(windowSize).ToArray();
            nextIndex = window.Length;
        }

        private void Reshuffle()
        {
            nextIndex = 0;
            rand.Shuffle(window.AsSpan());
        }

        private System.Random rand;
        private T[] window;
        private int nextIndex;
        /// <summary>
        /// Get next random element
        /// </summary>
        /// <returns>Next random element</returns>
        public T Next()
        {
            if (window.Length <= 0) return default;
            if (nextIndex >= window.Length)
                Reshuffle();
            return window[nextIndex++];
        }
    }
}