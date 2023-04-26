using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    public class BasicArmorPiece : IArmorPiece
    {
        public float MinDamage = 2f;
        public float DamageMultiplier = 1f;
        public override Damageable BaseDamageable => base.BaseDamageable ??= GetComponentInParent<Damageable>();

        protected override AttackDeclaration? ProcessAttackDeclaration(AttackDeclaration attack)
        {
            attack = attack.With(damage: attack.Damage * DamageMultiplier);
            if (attack.Damage < MinDamage) return null;
            return attack;
        }
    }
}