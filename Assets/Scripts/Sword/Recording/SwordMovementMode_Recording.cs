using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.Submodules;
using MarkusSecundus.PhysicsSwordfight.Sword;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Recording
{

    public class SwordMovementMode_Recording : SwordMovement.Submodule, ISwordMovement
    {
        public SwordDescriptor Sword => Script.Sword;
        public ISwordInput Input => Script.Input;
        public Transform SwordWielder => Script.SwordWielder;

        public UnityEvent<ISwordMovement.MovementCommand> MovementReporter = new UnityEvent<ISwordMovement.MovementCommand>();
        public ScriptSubmodulesContainer<KeyCode, SwordMovement.Submodule, ISwordMovement> Modes;
        IScriptSubmodule<ISwordMovement> submoduleManager;
        protected override void OnStart()
        {
            base.OnStart();
            submoduleManager = new ScriptSubmoduleListManager<KeyCode, SwordMovement.Submodule, ISwordMovement>() { ActivityPredicate = k => Input.GetKey(k), ModesSupplier = () => Modes }.Init(this);
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
}