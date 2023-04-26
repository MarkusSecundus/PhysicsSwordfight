using System;
using System.Collections;
using System.Collections.Generic;

namespace MarkusSecundus.Utils.Datastructs
{

    /// <summary>
    /// Decorator for IDictionary. Adds the functionality of creating default value when none is present but is being asked for.
    /// Similar to `defaultdict` in Python standard library.
    /// </summary>
    /// <typeparam name="K">
    /// Key datatype
    /// </typeparam>
    /// <typeparam name="V">
    /// Value datatype
    /// </typeparam>
    public struct DefaultValDict<K, V> : IDictionary<K, V>, IReadOnlyDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IReadOnlyCollection<KeyValuePair<K, V>>
    {
        /// <summary>
        /// The only valid constructor (unlike the default one)
        /// </summary>
        /// <param name="supplier">
        /// Supplier of default values. Gets passed the key for which the value is being asked.
        /// </param>
        /// <param name="baseDict">
        /// Object, to whom the new instance of <see cref="DefaultValDict{K, V}"/> will serve as decorator (by default <see cref="new Dictionary{K,V}()"/>)
        /// </param>
        public DefaultValDict(Func<K, V> supplier, IDictionary<K, V> baseDict = null)
        {
            this._supplier = supplier;
            this.Base = baseDict ?? new Dictionary<K, V>();
        }


        /*  The only really important piece of code in the whole of this class xD */
        public V this[K key]
        {
            get
            {
                if (Base.TryGetValue(key, out V ret))
                    return ret;
                else
                    return Base[key] = _supplier(key);
            }
            set => Base[key] = value;
        }

        /// <summary>
        /// Inner implementation being pointed to by this decorator.
        /// </summary>
        public readonly IDictionary<K, V> Base;

        /*funkce pro výchozí hodnotu mùže brát v úvahu i hodnotu tázaného klíèe*/
        private readonly Func<K, V> _supplier;




        //odsud níže je vše jenom boilerplate, pøesmìrovávající na _base

        public ICollection<K> Keys => Base.Keys;

        public ICollection<V> Values => Base.Values;

        public int Count => Base.Count;

        public bool IsReadOnly => Base.IsReadOnly;

        IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => Keys;

        IEnumerable<V> IReadOnlyDictionary<K, V>.Values => Values;

        public void Add(K key, V value) => Base.Add(key, value);

        public void Add(KeyValuePair<K, V> item) => Base.Add(item);

        public void Clear() => Base.Clear();

        public bool Contains(KeyValuePair<K, V> item) => Base.Contains(item);

        public bool ContainsKey(K key) => Base.ContainsKey(key);

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => Base.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => Base.GetEnumerator();

        public bool Remove(K key) => Base.Remove(key);

        public bool Remove(KeyValuePair<K, V> item) => Base.Remove(item);

        public bool TryGetValue(K key, out V value) => Base.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => Base.GetEnumerator();


        public override bool Equals(object obj) => Base.Equals(obj);

        public override int GetHashCode() => Base.GetHashCode();

        public override string ToString() => "default_dict.." + Base.ToString();

    }
}