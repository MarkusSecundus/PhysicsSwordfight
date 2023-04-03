using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SerializableDictionary
{
     public interface IEntry<TKey, TValue>
    {
        public TKey Key { get; init; }
        public TValue Value { get; init; }
    }

    [System.Serializable] public struct Entry<TKey, TValue> : IEntry<TKey, TValue>
    {
        [field: SerializeField] public TKey Key { get; init; }
        [field: SerializeField] public TValue Value { get; init; }
    }
}


[System.Serializable]
public class SerializableDictionary<TKey, TValue, TEntry> : ISerializationCallbackReceiver where TEntry: struct, SerializableDictionary.IEntry<TKey, TValue>
{
    [SerializeField] TEntry[] values = Array.Empty<TEntry>();

    public Dictionary<TKey, TValue> Values { get; } = new Dictionary<TKey, TValue>();

    public void OnBeforeSerialize(){}
    public void OnAfterDeserialize()
    {
        Values.Clear();
        FillDictionaryValues(Values);
    }

    protected virtual void FillDictionaryValues(Dictionary<TKey, TValue> dictionary)
    {
        foreach (var entry in values)
            dictionary.TryAdd(entry.Key, entry.Value);
    }
}
[System.Serializable] public class SerializableDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue, SerializableDictionary.Entry<TKey, TValue>> { }