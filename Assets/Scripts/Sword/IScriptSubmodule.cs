using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SwordMovement;

[System.Serializable]
public abstract class IScriptSubmodule<TScript>
{
    public TScript Script { get; private set; }

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
