using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;


namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    /// <summary>
    /// Component representing an entity that has healthpoints and thus can be damaged and die
    /// </summary>
    public abstract class IDamageable : MonoBehaviour
    {
        /// <summary>
        /// Healthpoints of the entity. When they reach 0, the entity dies.
        /// </summary>
        public abstract float HP { get; protected set; }
        /// <summary>
        /// Add or remove HP.
        /// 
        /// <para>Pass positive value to heal the entity, negative to damage it.</para>
        /// </summary>
        /// <param name="deltaHP">Ammount of HP to add.</param>
        public abstract void ChangeHP(float deltaHP);
    }

    /// <inheritdoc/>
    [Tooltip("Component representing an entity that has healthpoints and thus can be damaged and die")]
    public class Damageable : IDamageable
    {
        /// <summary>
        /// Callback type for all events fired by <see cref="Damageable"/>
        /// </summary>
        [System.Serializable] public class OnHpChangedEvent : UnityEvent<HpChangedArgs> { }

        /// <summary>
        /// Arguemnts provided to all events fired by <see cref="Damageable"/>
        /// </summary>
        [System.Serializable]
        public struct HpChangedArgs
        {
            /// <summary>
            /// The damageable whose HP changed
            /// </summary>
            [Tooltip("The damageable whose HP changed")]
            public Damageable Target;
            /// <summary>
            /// Change of HP. Positive means healing, negative means damage. 
            /// </summary>
            [Tooltip("Change of HP. Positive means healing, negative means damage")]
            public float DeltaHP;
        }

        /// <summary>
        /// Minimum ammount of HP - reaching it means death
        /// </summary>
        public const float MinHP = 0;
        /// <summary>
        /// Max amount of HP given entity can reach.
        /// </summary>
        [field: SerializeField, Tooltip("Max amount of HP given entity can reach.")] public float MaxHP { get; set; } = 1f;
        /// <summary>
        /// Current amount of HP of this entity.
        /// </summary>
        public override float HP { get; protected set; }

        /// <summary>
        /// Whether the entity should automatically destroy its gameobject right after firing <see cref="OnDeath"/> event. 
        /// 
        /// <para>For more complicated entites leave <c>false</c> to be able to respawn them or destroy them manually in controlled way.</para>
        /// </summary>
        [Tooltip("Whether the entity should automatically destroy its gameobject right after firing OnDeath event")]
        public bool DestroySelfOnDeath = false;

        /// <summary>
        /// Event fired just once right after the entity is spawned.
        /// </summary>
        [Tooltip("Event fired just once right after the entity is spawned")]
        [SerializeField] public OnHpChangedEvent OnSpawn;
        /// <summary>
        /// Event fired every time HP is changed - both on heal and damage.
        /// </summary>
        [Tooltip("Event fired every time HP is changed - both on heal and damage")]
        [SerializeField] public OnHpChangedEvent OnUpdated;
        /// <summary>
        /// Event fired every time HP decreases
        /// </summary>
        [Tooltip("Event fired every time HP decreases")]
        [SerializeField] public OnHpChangedEvent OnDamaged;
        /// <summary>
        /// Event fired every time HP increases
        /// </summary>
        [Tooltip("Event fired every time HP increases")]
        [SerializeField] public OnHpChangedEvent OnHealed;
        /// <summary>
        /// Event fired when the entity dies.
        /// 
        /// <para>
        /// If <see cref="DestroySelfOnDeath"/> is <c>false</c>, this event is reponsible for disposing of this entity, respawning it or something like that.
        /// </para>
        /// </summary>
        [Tooltip("Event fired when the entity dies")]
        [SerializeField] public OnHpChangedEvent OnDeath;

        void Start()
        {
            HP = MaxHP;
            var args = new HpChangedArgs { Target = this, DeltaHP = 0 };
            OnSpawn.Invoke(args);
            OnUpdated.Invoke(args);
        }

        /// <inheritdoc/>
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