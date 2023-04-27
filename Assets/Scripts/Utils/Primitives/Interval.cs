using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    [System.Serializable]
    public struct Interval<T>
    {
        public Interval(T min, T max) => (Min, Max) = (min, max);

        public T Min, Max;
    }

}
