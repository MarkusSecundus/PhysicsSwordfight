using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class IScriptSubmodule<TScript> where TScript: MonoBehaviour
{
    public TScript Script { get; private set; }

    protected Transform transform => Script.transform;
    protected GameObject gameObject => Script.gameObject;

    public IScriptSubmodule<TScript> Init(TScript script)
    {
        if (this.Script != null) Debug.LogError($"{this} was already initialized by '{this.Script}'!");
        this.Script = script;
        OnStart();
        return this;
    }
    protected virtual void OnStart() { }

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
