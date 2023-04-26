using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SwordMovement;

namespace MarkusSecundus.PhysicsSwordfight.Submodules
{

    [System.Serializable]
    public abstract class IScriptSubmodule<TScript>
    {
        public TScript Script { get; private set; }

        public IScriptSubmodule<TScript> Init(TScript script, bool forceReInit = false)
        {
            if (this.Script != null && !forceReInit)
                Debug.LogWarning($"Initializing {this} by {script} - but it was already initialized by '{this.Script}'!", script as Object);
            this.Script = script;
            OnStart(forceReInit);
            return this;
        }
        protected virtual void OnStart(bool wasForced = false) { }

        public virtual void OnUpdate(float delta) { }
        public virtual void OnFixedUpdate(float delta) { }

        public virtual void OnActivated() { }
        public virtual void OnDeactivated() { }

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