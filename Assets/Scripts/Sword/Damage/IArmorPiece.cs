using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    /// <summary>
    /// Component representing a set of colliders that create an attackable part of body of a <see cref="IDamageable"/>
    /// </summary>
    public interface IArmorPiece
    {
        /// <summary>
        /// Damageable entity this armor piece belongs to
        /// </summary>
        public IDamageable BaseDamageable { get; }
        /// <summary>
        /// Attacks <c>this</c> armor piece
        /// </summary>
        /// <param name="attack">Declaration of the attack</param>
        public void ProcessAttack(AttackDeclaration attack);

        /// <summary>
        /// Get appropriate armor piece for the given collider or conclude it doesn't belong to any armor piece
        /// </summary>
        /// <param name="c">Collider whose armor piece we want to obtain</param>
        /// <param name="ret">Found armor piece</param>
        /// <returns><c>true</c> IFF an armor piece was found</returns>
        public static bool TryGet(Collider c, out IArmorPiece ret)
        {
            ret = null;
            if (c && (ret = c.GetComponentInParent<IArmorPiece>()) != null)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Base class providing basic funcionality of <see cref="IArmorPiece"/>
    /// </summary>
    public abstract class ArmorPieceBase : MonoBehaviour, IArmorPiece
    {
        /// <summary>
        /// Event that is fired every time the armor piece is attacked
        /// </summary>
        [System.Serializable] public class OnAttackedEvent : UnityEvent<AttackDeclaration> { }
        /// <summary>
        /// Event that is fired every time the armor piece is attacked
        /// </summary>
        [Tooltip("Event that is fired every time the armor piece is attacked")]
        [SerializeField] OnAttackedEvent OnAttacked;

        /// <inheritdoc/>
        [field: Tooltip("Damageable entity this armor piece belongs to")]
        public IDamageable BaseDamageable { get; private set; }

        /// <summary>
        /// Process the raw attack declaration obtained from the weapon into a declaration that will actually be executed, according to armor stats.
        /// </summary>
        /// <param name="attack">Raw attack declaration obtained from attacking weapon</param>
        /// <returns>Final descriptor for the attack or <c>null</c> if the attack should be completely ignored</returns>
        protected abstract AttackDeclaration? ProcessAttackDeclaration(AttackDeclaration attack);


        void Start()
        {
            BaseDamageable = GetComponentInParent<IDamageable>();
        }


        /// <inheritdoc/>
        public void ProcessAttack(AttackDeclaration attack)
        {
            var processed = ProcessAttackDeclaration(attack);
            if (processed != null)
            {
                attack = processed.Value;
                OnAttacked.Invoke(attack);
                BaseDamageable.ChangeHP(-attack.Damage);
            }
        }
    }
}