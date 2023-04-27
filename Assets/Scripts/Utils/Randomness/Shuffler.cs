using MarkusSecundus.Utils.Datastructs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Randomness
{
    public struct Shuffler<T>
    {
        public IReadOnlyList<T> Items { get; }

        public Shuffler(System.Random randomizer, IReadOnlyList<T> items, int windowSize)
        {
            (Items, rand) = (items, randomizer);
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
        public T Next()
        {
            if (window.Length <= 0) return default;
            if (nextIndex >= window.Length)
                Reshuffle();
            return window[nextIndex++];
        }
    }
}