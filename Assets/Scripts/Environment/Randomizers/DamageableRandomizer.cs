using MarkusSecundus.PhysicsSwordfight.Automatization;
using MarkusSecundus.PhysicsSwordfight.Sword.Damage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Environment.Randomization
{

    [RequireComponent(typeof(Damageable))]
    public class DamageableRandomizer : MonoBehaviour, IRandomizer
    {
        public Interval<float> Hp;
        public void Randomize(System.Random random)
        {
            var dmg = GetComponent<Damageable>();
            dmg.MaxHP = random.Next(Hp);
        }
    }
}