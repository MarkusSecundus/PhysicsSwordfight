using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.Utils.Datastructs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Utils.Serialization
{

    /// <summary>
    /// Static class for utility functionalities concerning <see cref="SerializableDictionary{TKey, TValue, TEntry}"/>
    /// </summary>
    public class SerializableDictionary
    {
        /// <summary>
        /// Interface that all entry types of <see cref="SerializableDictionary{TKey, TValue, TEntry}"/> must fullfil.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        public interface IEntry<TKey, TValue>
        {
            /// <summary>
            /// Key for the dictionary
            /// </summary>
            public TKey Key { get; init; }
            /// <summary>
            /// Value
            /// </summary>
            public TValue Value { get; init; }
        }
    }


    /// <summary>
    /// Wrapper around a dictionary that can be used with Unity's builtin serializer
    /// </summary>
    /// <typeparam name="TKey">Key to be used in the dictionary</typeparam>
    /// <typeparam name="TValue">Value for dictionary elements</typeparam>
    /// <typeparam name="TEntry">Type used internally for the key-value pair struct that gets serialized. Defined as parameter to allow for adding custom attributes to key and value fields.</typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue, TEntry> : ISerializationCallbackReceiver where TEntry : struct, SerializableDictionary.IEntry<TKey, TValue>
    {
        /// <summary>
        /// Values of the dictionary
        /// </summary>
        [SerializeField] TEntry[] values = Array.Empty<TEntry>();
        /// <summary>
        /// Raw values of the dictionary - setting them directly doesn't lead to manual invocation of <see cref="OnAfterDeserialize"/>.
        /// </summary>
        protected Dictionary<TKey, TValue> _values = new Dictionary<TKey, TValue>();
        /// <summary>
        /// Values of the dictionary
        /// </summary>
        public IReadOnlyDictionary<TKey, TValue> Values { get => _values; set { values = value.Select(kv => new TEntry { Key = kv.Key, Value = kv.Value }).ToArray(); FillDictionaryValues(_values); } }

        /// <summary>
        /// Does nothing
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        /// <summary>
        /// Callback called by the serialization framework. Synchronizes values displayed in <see cref="Values"/> with their internal array representation.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            FillDictionaryValues(_values);
        }

        /// <summary>
        /// Synchronizes values of the provided dictionary <see cref="Values"/> with the internal array representation.
        /// </summary>
        /// <param name="dictionary">Dictionary to synchronize with the internal array representation.</param>
        protected virtual void FillDictionaryValues(Dictionary<TKey, TValue> dictionary)
        {
            _values.Clear();
            foreach (var entry in values)
                if(entry.Key.IsNotNil()) dictionary.TryAdd(entry.Key, entry.Value);
        }
    }

    /// <summary>
    /// Variant of <see cref="SerializableDictionary{TKey, TValue, TEntry}"/> that uses <see cref="SerializableDictionary.IEntry{TKey, TValue}"/> for its entries.
    /// </summary>
    /// <typeparam name="TKey">Key to be used in the dictionary</typeparam>
    /// <typeparam name="TValue">Value for dictionary elements</typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue, SerializableDictionary<TKey, TValue>.Entry>
    {
        [System.Serializable]
        public struct Entry : SerializableDictionary.IEntry<TKey, TValue>
        {
            [SerializeField] TKey key;
            [SerializeField] TValue value;
            public TKey Key { get => key; init => key = value; }  //for some god-known reason, [field: SerializeField] doesn't work here as it does in all other places
            public TValue Value { get => value; init => this.value = value; }
        }
    }

}