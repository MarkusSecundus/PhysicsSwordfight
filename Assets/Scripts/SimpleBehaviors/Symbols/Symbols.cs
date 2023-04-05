using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SymbolKey = System.String;


public interface ISymbol<TValue>
{
    public bool TryGet(out TValue ret);
}

[System.Serializable] public struct ComponentSymbol<TComponent> : ISymbol<TComponent> where TComponent: Component
{
    [SerializeField] SymbolMap Map;
    [SerializeField] SymbolKey Key;
    public bool TryGet(out TComponent ret) => Map.TryGet(Key, out ret);
}

[System.Serializable] public struct FloatSymbol : ISymbol<float>
{
    [SerializeField] SymbolMap Map;
    [SerializeField] SymbolKey Key;
    public bool TryGet(out float ret) => Map.TryGetFloat(Key, out ret);
}
