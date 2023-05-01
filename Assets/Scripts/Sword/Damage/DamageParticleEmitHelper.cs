using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    /// <summary>
    /// Cosmetic effect for emitting particles when a <see cref="Damageable"/> gets damaged or dies.
    /// 
    /// <para>Requires <see cref="ParticleSystem"/> to be present in the gameobject</para>
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class DamageParticleEmitHelper : MonoBehaviour
    {
        /// <summary>
        /// How many particles to emit every time the entity is damaged
        /// </summary>
        [SerializeField] public int ParticlesOnDamage = 3;
        /// <summary>
        /// How many particles to emit when the entity dies
        /// </summary>
        [SerializeField] public int ParticlesOnDeath = 20;

        ParticleSystem system;
        void Awake()
        {
            system = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// Emit small particle effect in the position where the entity was damaged
        /// </summary>
        /// <param name="attack"></param>
        public void EmitDamage(AttackDeclaration attack)
        {
            system.Emit(new ParticleSystem.EmitParams { position = attack.ImpactPoint.Point, applyShapeToPosition = true, velocity = attack.ImpactPoint.Normal.normalized * system.main.emitterVelocity.magnitude }, ParticlesOnDamage);
        }
        /// <summary>
        /// Emit big particle effect symbolizing that the entity just died
        /// </summary>
        /// <param name="makeSelfStandalone"></param>
        public void EmitDeath(bool makeSelfStandalone)
        {
            if (makeSelfStandalone)
                this.transform.SetParent(GameObjectHelpers.GetUtilObjectParent());
            system.Emit(ParticlesOnDeath);
            if (makeSelfStandalone)
                this.PerformWithDelay(() => Destroy(gameObject), new WaitWhile(() => system.isPlaying));
        }
    }
}