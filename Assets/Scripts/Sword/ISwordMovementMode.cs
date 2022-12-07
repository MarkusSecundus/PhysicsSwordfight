using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ISwordMovementMode
{
    public SwordMovement Script { get;  }

    protected Transform transform => Script.transform;
    protected GameObject gameObject => Script.gameObject;

    public ISwordMovementMode(SwordMovement script) => this.Script = script;

    public virtual void Update(float delta) { }
    public virtual void FixedUpdate(float delta) { }

    public virtual void OnActivated() { }
    public virtual void OnDeactivated() { }
}
