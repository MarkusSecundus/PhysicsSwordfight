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
}


[System.Serializable]
public class SerializableDictionary<TKey, TValue, TEntry> : ISerializationCallbackReceiver where TEntry: struct, SerializableDictionary.IEntry<TKey, TValue>
{
    [SerializeField] TEntry[] values = Array.Empty<TEntry>();

    Dictionary<TKey, TValue> _values = new Dictionary<TKey, TValue>();
    public IReadOnlyDictionary<TKey, TValue> Values { get=>_values; set { values = value.Select(kv=>new TEntry { Key=kv.Key, Value = kv.Value}).ToArray(); OnAfterDeserialize(); } } 

    public void OnBeforeSerialize(){}
    public void OnAfterDeserialize()
    {
        _values.Clear();
        FillDictionaryValues(_values);
    }

    protected virtual void FillDictionaryValues(Dictionary<TKey, TValue> dictionary)
    {
        foreach (var entry in values)
            dictionary.TryAdd(entry.Key, entry.Value);
    }
}
[System.Serializable] public class SerializableDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue, SerializableDictionary<TKey, TValue>.Entry>
{
    [System.Serializable]
    public struct Entry : SerializableDictionary.IEntry<TKey, TValue>
    {
        [SerializeField] TKey key;
        [SerializeField] TValue value;
        public TKey Key { get=>key; init=>key=value; }  //for some god-known reason, [field: SerializeField] doesn't work here as it does in all other places
        public TValue Value { get=>value; init=>this.value=value; }
    }
}