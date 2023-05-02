using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.Submodules;
using MarkusSecundus.PhysicsSwordfight.Sword;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Recording
{
    /// <summary>
    /// Decorator that slips between <see cref="ISwordMovement"/> and list of <see cref="SwordMovement.Module"/>s and reports all <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/> commands into a <see cref="UnityEvent"/>.
    /// </summary>
    public class SwordMovementMode_Recording : SwordMovement.Module, ISwordMovement
    {
        /// <inheritdoc/>
        public SwordDescriptor Sword => Script.Sword;
        /// <inheritdoc/>
        public ISwordInput Input => Script.Input;
        /// <inheritdoc/>
        public Transform SwordWielder => Script.SwordWielder;
        
        /// <summary>
        /// Callback invoked on each intercepted call to <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/>.
        /// </summary>
        public UnityEvent<ISwordMovement.MovementCommand> MovementReporter = new UnityEvent<ISwordMovement.MovementCommand>();

        /// <summary>
        /// Submodules responsible for actually controlling the sword. Their calls to <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/> get recorded.
        /// </summary>
        public ScriptSubmodulesContainer<KeyCode, SwordMovement.Module, ISwordMovement> Modes;
        IScriptSubmodule<ISwordMovement> submoduleManager;
        /// <summary>
        /// Inits <see cref="Modes"/> with <c>this</c> as their <see cref="IScriptSubmodule{TScript}.Script"/>
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            submoduleManager = new ScriptSubmoduleListManager<KeyCode, SwordMovement.Module, ISwordMovement>() { ActivityPredicate = k => Input.GetKey(k), ModesSupplier = () => Modes }.Init(this);
        }
        /// <summary>
        /// Calls <see cref="IScriptSubmodule{TScript}.OnUpdate(float)"/> on the currently active submodule.
        /// </summary>
        /// <param name="delta">Time elapsed from last <see cref="IScriptSubmodule{TScript}.OnUpdate(float)"/> call</param>
        public override void OnUpdate(float delta) => submoduleManager.OnUpdate(delta);
        /// <summary>
        /// Calls <see cref="IScriptSubmodule{TScript}.OnDrawGizmos(float)"/> on the currently active submodule.
        /// </summary>
        public override void OnDrawGizmos() => submoduleManager?.OnDrawGizmos();
        /// <summary>
        /// Calls <see cref="IScriptSubmodule{TScript}.OnFixedUpdate(float)"/> on the currently active submodule.
        /// </summary>
        /// <param name="delta">Time elapsed from last <see cref="IScriptSubmodule{TScript}.OnFixedUpdate(float)"/> call</param>
        public override void OnFixedUpdate(float delta) => submoduleManager.OnFixedUpdate(Time.fixedDeltaTime);

        /// <summary>
        /// Invoke <see cref="MovementReporter"/> and then call <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/> on the base.
        /// </summary>
        /// <param name="m">Where the sword should move to</param>
        public void MoveSword(ISwordMovement.MovementCommand m)
        {
            MovementReporter.Invoke(m);
            Script.MoveSword(m);
        }
    }
}