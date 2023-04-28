using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;


namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    public abstract class IDamageable : MonoBehaviour
    {
        public abstract float HP { get; protected set; }
        public abstract void ChangeHP(float deltaHP);
    }

    public class Damageable : IDamageable
    {
        [System.Serializable] public class OnHpChangedEvent : UnityEvent<HpChangedArgs> { }

        [System.Serializable]
        public struct HpChangedArgs
        {
            public Damageable Target;
            public float DeltaHP;
        }

        public const float MinHP = 0;
        [field: SerializeField] public float MaxHP { get; set; } = 1f;
        public override float HP { get; protected set; }

        public bool DestroySelfOnDeath = false;

        [SerializeField] public OnHpChangedEvent OnSpawn;
        [SerializeField] public OnHpChangedEvent OnUpdated;
        [SerializeField] public OnHpChangedEvent OnDamaged;
        [SerializeField] public OnHpChangedEvent OnHealed;
        [SerializeField] public OnHpChangedEvent OnDeath;

        private void Start()
        {
            HP = MaxHP;
            var args = new HpChangedArgs { Target = this, DeltaHP = 0 };
            OnSpawn.Invoke(args);
            OnUpdated.Invoke(args);
        }

        public override void ChangeHP(float deltaHP)
        {
            HP = Mathf.Clamp(HP + deltaHP, MinHP, MaxHP);

            var eventArgs = new HpChangedArgs { Target = this, DeltaHP = deltaHP };

            if (deltaHP < 0) OnDamaged.Invoke(eventArgs);
            else if (deltaHP > 0) OnHealed.Invoke(eventArgs);
            OnUpdated.Invoke(eventArgs);
            if (HP <= 0)
            {
                OnDeath.Invoke(eventArgs);
                if (DestroySelfOnDeath)
                    Destroy(gameObject);
            }
        }
    }
}