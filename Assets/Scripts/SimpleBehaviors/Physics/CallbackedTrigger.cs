using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using callback = UnityEngine.Events.UnityAction<UnityEngine.Collider>;


namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    /// <summary>
    /// Component that provides callbacks to be invoked on <c>OnTriggerEnter</c> and <c>OnTriggerExit</c> signals.
    /// </summary>
    public class CallbackedTrigger : MonoBehaviour
    {
        /// <summary>
        /// Action to be invoked when <c>OnTriggerEnter</c> message is received
        /// </summary>
        public UnityEvent<Collider> OnEnter = new UnityEvent<Collider>();
        /// <summary>
        /// Action to be invoked when <c>OnTriggerExit</c> message is received
        /// </summary>
        public UnityEvent<Collider> OnExit = new UnityEvent<Collider>();

        private IEnumerable<Collider> colliders;
        /// <summary>
        /// All trigger colliders of this gameobject
        /// </summary>
        public IEnumerable<Collider> Colliders => colliders ??= GetComponents<Collider>().Where(c=>c.isTrigger).ToArray();

        /// <summary>
        /// Add a triggercollider to <c>this</c> gameobject easily from a script
        /// </summary>
        /// <typeparam name="T">Type of triggercollider to by instantiated</typeparam>
        /// <param name="initializer">Action to be invoked on the newly created collider to initialize it</param>
        /// <returns><c>this</c> for chaining purposes</returns>
        public CallbackedTrigger Add<T>(System.Action<T> initializer) where T : Collider
        {
            var collider = gameObject.AddComponent<T>();
            collider.isTrigger = true;
            initializer?.Invoke(collider);

            return this;
        }

        /// <summary>
        /// Shortcut for initializing all the important aspects of this component from a script.
        /// </summary>
        /// <param name="layer"><see cref="ColliderLayer"/> to be set for <c>this</c> gameobject</param>
        /// <param name="onEnter">Callback to be added to <see cref="OnEnter"/></param>
        /// <param name="onExit">Callback to be added to <see cref="OnExit"/></param>
        /// <returns><c>this</c> for chaining purposes</returns>
        public CallbackedTrigger Init(int layer, callback onEnter = null, callback onExit = null)
        {
            gameObject.layer = layer;
            if(onEnter.IsNotNil()) OnEnter.AddListener(onEnter);
            if(onExit.IsNotNil()) OnExit.AddListener(onExit);

            return this;
        }

        void OnTriggerEnter(Collider other) => OnEnter?.Invoke(other);
        void OnTriggerExit(Collider other) => OnExit?.Invoke(other);
    }
}