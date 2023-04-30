using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Submodules
{

    /// <summary>
    /// Lightweight piece of functionality that doesn't exist as standalone but gets invoked by a heavyweight gameobject in controlled way.
    /// </summary>
    /// <typeparam name="TScript">Type of the parent gameobject that provides access to external functionality.</typeparam>
    [System.Serializable]
    public abstract class IScriptSubmodule<TScript>
    {
        /// <summary>
        /// Reference to the parent gameobject that provides access to external functionality.
        /// </summary>
        public TScript Script { get; private set; }

        /// <summary>
        /// Initialize the submodule´with the provided instance of parent game object.
        /// </summary>
        /// <param name="script">
        /// Reference to the parent gameobject that provides access to external functionality.</param>
        /// <returns>Reference to<c>this</c> for chaining purposes</returns>
        public IScriptSubmodule<TScript> Init(TScript script)
        {
            this.Script = script;
            OnStart();
            return this;
        }
        /// <summary>
        /// Called one time just after the Script reference is set. Should be used for acquiring resources and overall initialization.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Callback to update game state. Called each frame (as the Update() message in <see cref="MonoBehaviour"/>).
        /// </summary>
        /// <param name="delta">Time passed from last <see cref="OnUpdate(float)"/> call</param>
        public virtual void OnUpdate(float delta) { }

        /// <summary>
        /// Callback to update game state. Called in fixed intervals (as the FixedUpdate() message in <see cref="MonoBehaviour"/>).
        /// </summary>
        /// <param name="delta">Time passed from last <see cref="OnFixedUpdate(float)"/> call</param>
        public virtual void OnFixedUpdate(float delta) { }

        /// <summary>
        /// Callback signalling the component is now active and will receive <see cref="OnUpdate(float)"/> and <see cref="OnFixedUpdate(float)"/> callbacks.until deactivated.
        /// </summary>
        public virtual void OnActivated() { }
        /// <summary>
        /// Callback signalling the component was just set as non-active and will no longer receive <see cref="OnUpdate(float)"/> and <see cref="OnFixedUpdate(float)"/> callbacks until again activated.
        /// </summary>
        public virtual void OnDeactivated() { }

        /// <summary>
        /// Callback called during the gizmos draw update. (as OnDrawGizmos() message in <see cref="MonoBehaviour"/>)
        /// </summary>
        public virtual void OnDrawGizmos() { }
        /*
        public virtual void OnCollisionEnter(Collision col) { }
        public virtual void OnCollisionStay(Collision col) { }
        public virtual void OnCollisionExit(Collision col) { }
        public virtual void OnTriggerEnter(Collider other) { }
        public virtual void OnTriggerStay(Collider other) { }
        public virtual void OnTriggerExit(Collider other) { }*/
    }
}