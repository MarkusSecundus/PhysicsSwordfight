using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SymbolKey = System.String;


public interface ISymbol<TValue>
{
    public TValue Get();
}

[System.Serializable] public struct ComponentSymbol<TComponent> : ISymbol<TComponent> where TComponent: Component
{
    [SerializeField] SymbolMap Map;
    [SerializeField] SymbolKey Key;
    [SerializeField] public TComponent Default;
    public TComponent Get() => Map.TryGet<TComponent>(Key, out var ret)?ret:Default;
}

[System.Serializable] public struct FloatSymbol : ISymbol<float>
{
    [SerializeField] SymbolMap Map;
    [SerializeField] SymbolKey Key;
    [SerializeField] public float Default;
    public float Get() => Map.TryGetFloat(Key, out var ret)?ret:Default;
}
