using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SymbolKey = System.String;

namespace MarkusSecundus.PhysicsSwordfight.Symbols
{
    /// <summary>
    /// Reference to a value defined in a <see cref="SymbolMap"/>
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface ISymbol<TValue>
    {
        /// <summary>
        /// Get the value of this symbol
        /// </summary>
        /// <returns>Value of this symbol</returns>
        public TValue Get();
    }

    /// <summary>
    /// Reference to a <see cref="Component"/> defined in a <see cref="SymbolMap"/>
    /// </summary>
    /// <typeparam name="TComponent">Type of the component value</typeparam>
    [System.Serializable]
    public struct ComponentSymbol<TComponent> : ISymbol<TComponent> where TComponent : Component
    {
        /// <summary>
        /// Map containing the symbol definition
        /// </summary>
        [SerializeField] SymbolMap Map;
        /// <summary>
        /// Name of the symbolic value
        /// </summary>
        [SerializeField] SymbolKey Key;
        /// <summary>
        /// Default value to be used if no 
        /// </summary>
        [SerializeField] public TComponent Default;
        public TComponent Get() => Map.IsNotNil()&&Map.TryGet<TComponent>(Key, out var ret) ? ret : Default;
    }

    [System.Serializable]
    public struct FloatSymbol : ISymbol<float>
    {
        [SerializeField] SymbolMap Map;
        [SerializeField] SymbolKey Key;
        [SerializeField] public float Default;
        public float Get() => Map.TryGetFloat(Key, out var ret) ? ret : Default;
    }

}