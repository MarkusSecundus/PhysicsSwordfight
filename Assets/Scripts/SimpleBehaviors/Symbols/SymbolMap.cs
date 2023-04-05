using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

using SymbolKey = System.String;

public class SymbolMap : MonoBehaviour
{
    public static SymbolMap Get(GameObject o) => o.GetComponentInParent<SymbolMap>();

    [SerializeField] SerializableDictionary<SymbolKey, Component> Components;
    [SerializeField] SerializableDictionary<SymbolKey, float> Floats;

    public bool TryGet<TComponent>(SymbolKey key, out TComponent ret) where TComponent : Component
    {
        ret = default;
        return (Components.Values.TryGetValue(key, out var retComponent) && (ret = retComponent as TComponent) != null) ;
    }
    public bool TryGetFloat(SymbolKey key, out float ret) => Floats.Values.TryGetValue(key, out ret);



    System.Action<string> logError => s => Debug.LogError(s, this);
    public void IndirectMessage(string calleeAndMessage, bool calleeMustExist)
    {
        if (IndirectionUtils.IndirectMessage.Make(calleeAndMessage, logError) is IndirectionUtils.IndirectMessage message)
        {
            if (TryGet<Component>(message.CalleeName, out var callee))
            {
                message.Invoke(callee, logError);
            }
            else if (calleeMustExist)
                Debug.LogError($"Failed sending message '{calleeAndMessage}' - no callee '{message.CalleeName}' was found", this);
        }
    }

    public void IndirectMessage(string calleeAndMessage) => IndirectMessage(calleeAndMessage, true);
    public void TryIndirectMessage(string calleeAndMessage) => IndirectMessage(calleeAndMessage, false);

#if false
    [SerializeField] SerializableDictionary<SymbolKey, string> Strings;
    [SerializeField] SerializableDictionary<SymbolKey, int> Integers;
    [SerializeField] SerializableDictionary<SymbolKey, Vector3> Vectors;

    public bool TryGetString(SymbolKey key, out string ret) => Strings.Values.TryGetValue(key, out ret);
    public bool GetInteger(SymbolKey key, out int ret) => Integers.Values.TryGetValue(key, out ret);
    public bool GetVector(SymbolKey key, out Vector3 ret) => Vectors.Values.TryGetValue(key, out ret);
#endif
}

