using MarkusSecundus.PhysicsSwordfight.Automatization;
using MarkusSecundus.PhysicsSwordfight.Sword.Damage;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.PhysicsSwordfight.Utils.Randomness;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Environment.Randomization
{
    /// <summary>
    /// Randomizes stats of a <see cref="Damageable"/>
    /// </summary>
    [RequireComponent(typeof(Damageable))]
    public class DamageableRandomizer : MonoBehaviour, IRandomizer
    {
        /// <summary>
        /// Health Points of the damageable
        /// </summary>
        public Interval<float> Hp;
        /// <inheritdoc/>
        public void Randomize(System.Random random)
        {
            var dmg = GetComponent<Damageable>();
            dmg.MaxHP = random.Next(Hp);
        }
    }
}