using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    /// <summary>
    /// Base class for weapons that attack by colliding with other objects.
    /// 
    /// <para>See <see cref="IDamageable"/>, <see cref="IArmorPiece"/>.</para>
    /// </summary>
    /// <typeparam name="TStats">Stats describing the weapon</typeparam>
    public abstract class ImpactWeapon<TStats> : MonoBehaviour
    {
        /// <summary>
        /// Minimal interval between two attacks on the same <see cref="IDamageable"/> entity being registered
        /// </summary>
        [Tooltip("Minimal interval between two attacks on the same IDamageable entity being registered")]
        public float SecondsBetweenAttacks = 0.2f;

        /// <summary>
        /// Stats describing the weapon
        /// </summary>
        [Tooltip("Stats describing the weapon")]
        public TStats Stats;

        /// <summary>
        /// List of specific <see cref="IDamageable"/>s  agains which special stats should be used
        /// </summary>
        [Tooltip("List of specific IDamageables agains which special stats should be used")]
        public ExceptionsList Exceptions;

        /// <summary>
        /// List of specific <see cref="IDamageable"/>s  agains which special stats should be used
        /// </summary>
        [System.Serializable] public class ExceptionsList : SerializableDictionary<IDamageable, TStats> { }

        /// <summary>
        /// Calculate attack declaration according to the occured collision and stats
        /// </summary>
        /// <param name="collision">Collision that initiated the attack</param>
        /// <param name="stats">Stats to be used against the attacked entity</param>
        /// <returns>Declaration of the attack or <c>null</c> if the attack should be cancelled instead</returns>
        protected abstract AttackDeclaration? CalculateAttackStats(Collision collision, TStats stats);


        readonly HashSet<IArmorPiece> targeted = new HashSet<IArmorPiece>();

        void OnCollisionEnter(Collision collision)
        {
            if (!IArmorPiece.TryGet(collision.collider, out var hit)) return;
            if (targeted.TryGetValue(hit, out _)) return;
            targeted.Add(hit);
            ProcessCollision(collision, hit);
        }
        void OnCollisionExit(Collision collision)
        {
            if (!IArmorPiece.TryGet(collision.collider, out var hit)) return;
            this.PerformWithDelay(() => targeted.Remove(hit), SecondsBetweenAttacks);
        }

        void ProcessCollision(Collision collision, IArmorPiece hit)
        {
            var statsToUse = Exceptions.Values.GetValueOrDefault(hit.BaseDamageable, Stats);
            var attack = CalculateAttackStats(collision, statsToUse);
            if(attack != null)
                hit.ProcessAttack(attack.Value);
        }
    }

    /// <summary>
    /// Component describing a simple blunt weapon that attacks <see cref="IArmorPiece"/>s by colliding with them and does the same ammount of damage no matter which part of the weapon performed the hit.
    /// </summary>
    public class BasicImpactWeapon : ImpactWeapon<BasicImpactWeapon.StatsDefinition>
    {
        /// <summary>
        /// Stats describing the weapon
        /// </summary>
        [System.Serializable]
        public struct StatsDefinition
        {
            /// <summary>
            /// Value with which <c><see cref="Collision.impulse"/>.magnitude</c> is multiplied to get total attack damage.
            /// </summary>
            [Tooltip("Value with which Collision.impulse.magnitude is multiplied to get total attack damage")]
            public float DamageMultiplier;
        }
        /// <inheritdoc/>
        protected override AttackDeclaration? CalculateAttackStats(Collision collision, StatsDefinition stats) => new AttackDeclaration
        {
            Damage = collision.impulse.magnitude * stats.DamageMultiplier,
            AttackerIdentifier = this,
            ImpactPoint = collision.GetContact(0).AsImpactPointData()
        };
    }
}