using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Submodules
{

    /// <summary>
    /// Container carrying a set of <see cref="IScriptSubmodule{TScript}"/>s - one default and multiple indexed ones
    /// </summary>
    /// <typeparam name="TKey">Identifier type for the modules</typeparam>
    /// <typeparam name="TSubmodule">Base type for all the submodules stored here</typeparam>
    /// <typeparam name="TScript">Type parameter of <typeparamref name="TSubmodule"/></typeparam>
    [System.Serializable]
    public class ScriptSubmodulesContainer<TKey, TSubmodule, TScript> : SerializableDictionary<TKey, TSubmodule, ScriptSubmodulesContainer<TKey, TSubmodule, TScript>.Entry> where TSubmodule : IScriptSubmodule<TScript>
    {
        /// <summary>
        /// Entry to be used for storing the <typeparamref name="TKey"/>-<typeparamref name="TSubmodule"/> pairs inside <see cref="SerializableDictionary{TKey, TValue, TEntry}"/>
        /// </summary>
        [System.Serializable]
        public struct Entry : SerializableDictionary.IEntry<TKey, TSubmodule>
        {
            [SerializeField] internal TKey Key;
            [SerializeField][SerializeReference][Subclass] internal TSubmodule Value;
            /// <inheritdoc/>
            TKey SerializableDictionary.IEntry<TKey, TSubmodule>.Key { get => this.Key; init => this.Key = value; }
            /// <inheritdoc/>
            TSubmodule SerializableDictionary.IEntry<TKey, TSubmodule>.Value { get => this.Value; init => this.Value = value; }
        }

        [SerializeField, SerializeReference, Subclass] private TSubmodule _default;
        /// <summary>
        /// Submodule that is active by default when no other module is active
        /// </summary>
        public TSubmodule Default { get => _default; set { _default = value; FillDictionaryValues(this._values); } }

        /// <inheritdoc/>
        protected override void FillDictionaryValues(Dictionary<TKey, TSubmodule> dictionary)
        {
            base.FillDictionaryValues(dictionary);
            dictionary[default] = Default;
        }
    }

    /// <summary>
    /// Object responsible for managing a set of <see cref="IScriptSubmodule{TScript}"/>s stored in a <see cref="ScriptSubmodulesContainer{TKey, TSubmodule, TScript}"/>. Manages activity of the submodules and calls update callbacks on the active one.
    /// </summary>
    /// <typeparam name="TKey">Identifier type for the modules</typeparam>
    /// <typeparam name="TSubmodule">Base type for all the submodules stored here</typeparam>
    /// <typeparam name="TScript">Type parameter of <typeparamref name="TSubmodule"/></typeparam>
    public class ScriptSubmoduleListManager<TKey, TSubmodule, TScript> : IScriptSubmodule<TScript> where TSubmodule : IScriptSubmodule<TScript>
    {
        /// <summary>
        /// Indirect supplier for the managed modes container. Value provided by the supplier is allowed to change at any moment
        /// </summary>
        public System.Func<ScriptSubmodulesContainer<TKey, TSubmodule, TScript>> ModesSupplier { protected get; init; }
        /// <summary>
        /// Function for determining which submodule is active.
        /// </summary>
        public System.Func<TKey, bool> ActivityPredicate { protected get; init; }

        TSubmodule activeMode;
        /// <summary>
        /// Inits all child submodules
        /// </summary>
        protected override void OnStart()
        {
            foreach (var mode in ModesSupplier().Values.Values) mode.Init(Script);
        }

        /// <summary>
        /// Makes sure the right submodule is active and calls <see cref="OnUpdate(float)"/> on it.
        /// </summary>
        /// <param name="delta">Time elapsed from last <see cref="OnUpdate(float)"/> call</param>
        public override void OnUpdate(float delta)
        {
            MakeSureRightModeIsActive();
            activeMode?.OnUpdate(Time.deltaTime);
        }
        /// <summary>
        /// Makes sure the right submodule is active and calls <see cref="OnFixedUpdate(float)"/> on it.
        /// </summary>
        /// <param name="delta">Time elapsed from last <see cref="OnFixedUpdate(float)"/> call</param>
        public override void OnFixedUpdate(float delta)
        {
            MakeSureRightModeIsActive();
            activeMode?.OnFixedUpdate(delta);
        }
        /// <summary>
        /// Calls <see cref="OnDrawGizmos"/> on the active submodule.
        /// </summary>
        public override void OnDrawGizmos() => activeMode?.OnDrawGizmos();
        void MakeSureRightModeIsActive()
        {
            var modes = this.ModesSupplier();
            var mode = modes.Values[modes.Values.Keys.FirstOrDefault(ActivityPredicate)];
            if (activeMode != mode)
            {
                activeMode?.OnDeactivated();
                (activeMode = mode)?.OnActivated();
            }
        }
    }
}