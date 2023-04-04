using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SwordMovement;

public class SwordMovementMode_Recording : SwordMovement.Submodule, ISwordMovement
{
    public SwordDescriptor Sword => Script.Sword;
    public ISwordInput Input => Script.Input;
    
    public UnityEvent<ISwordMovement.MovementCommand> MovementReporter = new UnityEvent<ISwordMovement.MovementCommand>();
    public ScriptSubmodulesContainer<KeyCode, Submodule, ISwordMovement> Modes;
    IScriptSubmodule<ISwordMovement> submoduleManager;
    protected override void OnStart(bool wasForced)
    {
        base.OnStart(wasForced);
        submoduleManager = new ScriptSubmoduleListManager<KeyCode, Submodule, ISwordMovement>() { ActivityPredicate = Input.GetKey, ModesSupplier =()=> Modes }.Init(this, wasForced);
    }
    public override void OnUpdate(float delta) => submoduleManager.OnUpdate(delta);
    public override void OnDrawGizmos() => submoduleManager?.OnDrawGizmos();
    public override void OnFixedUpdate(float delta) => submoduleManager.OnFixedUpdate(Time.fixedDeltaTime);

    public void MoveSword(ISwordMovement.MovementCommand m)
    {
        MovementReporter.Invoke(m);
        Script.MoveSword(m);
    }
}
