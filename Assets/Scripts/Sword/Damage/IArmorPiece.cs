using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    public abstract class IArmorPiece : MonoBehaviour
    {
        [System.Serializable] public class OnAttackedEvent : UnityEvent<AttackDeclaration> { }


        public IDamageable BaseDamageable { get; private set; }
        protected abstract AttackDeclaration? ProcessAttackDeclaration(AttackDeclaration attack);

        [SerializeField] OnAttackedEvent OnAttacked;

        private void Start()
        {
            BaseDamageable = GetComponentInParent<IDamageable>();
        }

        public static bool TryGet(Collider c, out IArmorPiece ret)
        {
            ret = null;
            if (c && (ret = c.GetComponentInParent<IArmorPiece>()) != null)
                return true;
            return false;
        }

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