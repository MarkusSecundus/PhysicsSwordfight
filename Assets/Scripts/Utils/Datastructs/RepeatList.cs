using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkusSecundus.Utils.Datastructs
{
    public class RepeatList<T> : IReadOnlyList<T>
    {
        public RepeatList(T value, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), $"Must not be negative number (but is {count})");

            (Value, Count) = (value, count);
        }


        public T Value { get; }

        public T this[int index] => index < 0 || index >= Count ? throw new IndexOutOfRangeException($"Index {index} out of range [0,{Count})") : Value;

        public int Count { get; }

        public IEnumerator<T> GetEnumerator()
        {
            for (int t = Count; --t >= 0;) yield return Value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
