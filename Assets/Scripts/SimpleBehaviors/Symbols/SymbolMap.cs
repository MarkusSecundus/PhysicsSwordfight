using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

using SymbolKey = System.String;

namespace MarkusSecundus.PhysicsSwordfight.Symbols
{
    /// <summary>
    /// Component for defining named variables that can be referenced indirectly by name from other components.
    /// </summary>
    public class SymbolMap : MonoBehaviour
    {
        /// <summary>
        /// Finds the cannonical symbol map for the provided gameobject - currently the one found by <see cref="Component.GetComponentInParent{T}()"/>
        /// </summary>
        /// <param name="o">Game object that's searching for its symbol map.</param>
        /// <returns>SymbolMap to be used by the game object</returns>
        public static SymbolMap Get(GameObject o) => o.GetComponentInParent<SymbolMap>();

        [SerializeField] SerializableDictionary<SymbolKey, Component> Components;
        [SerializeField] SerializableDictionary<SymbolKey, float> Floats;

        /// <summary>
        /// Try to get a component by name
        /// </summary>
        /// <typeparam name="TComponent">Type of the component to be found</typeparam>
        /// <param name="key">Name of the symbol</param>
        /// <param name="ret">Found component</param>
        /// <returns><c>true</c> IFF the symbol was successfully found</returns>
        public bool TryGet<TComponent>(SymbolKey key, out TComponent ret) where TComponent : Component
        {
            ret = default;
            return (Components.Values.TryGetValue(key, out var retComponent) && ((ret = retComponent as TComponent).IsNotNil() || (retComponent.IsNotNil() && (ret = retComponent.GetComponent<TComponent>()).IsNotNil() )));
        }
        /// <summary>
        /// Try to get a number by name
        /// </summary>
        /// <param name="key">Name of the symbol</param>
        /// <param name="ret">Found number</param>
        /// <returns><c>true</c> IFF the symbol was successfully found</returns>
        public bool TryGetFloat(SymbolKey key, out float ret) => Floats.Values.TryGetValue(key, out ret);



        System.Action<string> logError => s => Debug.LogError(s, this);

        /// <summary>
        /// Finds a component by its name and invokes a specified method on it.
        /// 
        /// Intended to be used as a UnityEvent callback. Receives one argument - string in format <c>"ComponentName.MethodName"</c>. The method MUST NOT have overloads.
        /// </summary>
        /// <param name="calleeAndMessage">String in format <c>"ComponentName.MethodName"</c></param>
        /// <param name="calleeMustExist">Whether to log an error if no callee was found</param>
        /// <param name="argument">Object to provide as the message's argument. If null, the message is assumed to have 0 arguments.</param>
        public void IndirectMessage(string calleeAndMessage, bool calleeMustExist, object argument=null)
        {
            if (GameObjectHelpers.IndirectMessage.Make(calleeAndMessage, logError) is GameObjectHelpers.IndirectMessage message)
            {
                if (TryGet<Component>(message.CalleeName, out var callee))
                {
                    message.Argument = argument;
                    message.Invoke(callee, logError);
                }
                else if (calleeMustExist)
                    Debug.LogError($"Failed sending message '{calleeAndMessage}' - no callee '{message.CalleeName}' was found", this);
            }
        }

        /// <summary>
        /// Finds a component by its name and invokes a specified method on it.
        /// 
        /// Logs an error if no callee was found.
        /// Intended to be used as a UnityEvent callback. Receives one argument - string in format <c>"ComponentName.MethodName"</c>. The method MUST NOT have overloads.
        /// </summary>
        /// <param name="calleeAndMessage">String in format <c>"ComponentName.MethodName"</c></param>
        public void IndirectMessage(string calleeAndMessage) => IndirectMessage(calleeAndMessage, true);

        /// <summary>
        /// Finds a component by its name and invokes a specified method on it.
        /// 
        /// Silently returns if no callee was found.
        /// Intended to be used as a UnityEvent callback. Receives one argument - string in format <c>"ComponentName.MethodName"</c>. The method MUST NOT have overloads.
        /// </summary>
        /// <param name="calleeAndMessage">String in format <c>"ComponentName.MethodName"</c></param>
        public void TryIndirectMessage(string calleeAndMessage) => IndirectMessage(calleeAndMessage, false);
        public void TryIndirectMessageWithComponent(string calleeAndMessage, Component component) => IndirectMessage(calleeAndMessage, false, argument:component);

#if false
    [SerializeField] SerializableDictionary<SymbolKey, string> Strings;
    [SerializeField] SerializableDictionary<SymbolKey, int> Integers;
    [SerializeField] SerializableDictionary<SymbolKey, Vector3> Vectors;

    public bool TryGetString(SymbolKey key, out string ret) => Strings.Values.TryGetValue(key, out ret);
    public bool GetInteger(SymbolKey key, out int ret) => Integers.Values.TryGetValue(key, out ret);
    public bool GetVector(SymbolKey key, out Vector3 ret) => Vectors.Values.TryGetValue(key, out ret);
#endif
    }

}