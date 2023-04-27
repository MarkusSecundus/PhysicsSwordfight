using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    [RequireComponent(typeof(ParticleSystem))]
    public class DamageParticleEmitHelper : MonoBehaviour
    {
        [SerializeField] public int ParticlesOnDamage = 3, ParticlesOnDeath = 20;

        ParticleSystem system;
        private void Awake()
        {
            system = GetComponent<ParticleSystem>();
        }

        public void EmitDamage(AttackDeclaration attack)
        {
            system.Emit(new ParticleSystem.EmitParams { position = attack.ImpactPoint.Point, applyShapeToPosition = true, velocity = attack.ImpactPoint.Normal.normalized * system.main.emitterVelocity.magnitude }, ParticlesOnDamage);
        }
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