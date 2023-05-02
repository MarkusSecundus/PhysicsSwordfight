using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    /// <summary>
    /// Basic piece of armor that can ignore weak attacks under certain threshold and increase/decrease damage dealt according to a multiplier
    /// </summary>
    public class BasicArmorPiece : ArmorPieceBase
    {
        /// <summary>
        /// Minimum damage for an attack to not be ignored
        /// </summary>
        public float MinDamage = 2f;
        /// <summary>
        /// Value to multiply the original attack damage obtained from the weapon
        /// </summary>
        public float DamageMultiplier = 1f;

        /// <inheritdoc/>
        protected override AttackDeclaration? ProcessAttackDeclaration(AttackDeclaration attack)
        {
            attack.Damage *= DamageMultiplier;
            if (attack.Damage < MinDamage) return null;
            return attack;
        }
    }
}