using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [System.Serializable] struct Entry
    {
        public TKey Key;
        public TValue Value;
    }
    [SerializeField] Entry[] values = Array.Empty<Entry>();


    [NonSerialized] bool isDirty = false;

    [NonSerialized] Dictionary<TKey, TValue> valuesRaw = new Dictionary<TKey, TValue>();
    public Dictionary<TKey, TValue> Values { get { isDirty = true; return valuesRaw; } }

    public void OnBeforeSerialize()
    {
        if (Op.post_assign(ref isDirty, false))
            values = valuesRaw.Select(kv => new Entry { Key = kv.Key, Value = kv.Value }).ToArray();
    }
    public void OnAfterDeserialize()
    {
        valuesRaw.Clear();
        FillDictionaryValues(valuesRaw);
    }

    protected virtual void FillDictionaryValues(Dictionary<TKey, TValue> dictionary)
    {
        foreach (var entry in values)
            dictionary.TryAdd(entry.Key, entry.Value);
    }
}
