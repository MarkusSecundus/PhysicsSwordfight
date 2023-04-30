using MarkusSecundus.PhysicsSwordfight.Sword.Damage;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Args = MarkusSecundus.PhysicsSwordfight.Sword.Damage.Damageable.HpChangedArgs;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// Component responsible for visualizing the state of single <see cref="Damageable"/> on the HUD (healthbar + bloodstains).
    /// </summary>
    public class DamageHUD : MonoBehaviour
    {
        /// <summary>
        /// Finds the cannonical damage hud for the provided gameobject - currently always a singleton found by <see cref="Object.FindAnyObjectByType{T}"/>
        /// </summary>
        /// <param name="self">Game object that's searching for its damage hud.</param>
        /// <returns>Damage hud to be used by the gameobject.</returns>
        public static DamageHUD Get(GameObject self) => GameObject.FindAnyObjectByType<DamageHUD>();

        /// <summary>
        /// The healthbar to be used
        /// </summary>
        [SerializeField] ResourceBar HealthBar;
        /// <summary>
        /// The bloodstains manager to be used
        /// </summary>
        [SerializeField] BloodStains BloodstainsInstancer;

        /// <summary>
        /// The damageable object whose state should be visualized
        /// </summary>
        [SerializeField] private Damageable _target;

        /// <summary>
        /// The damageable object whose state should be visualized
        /// </summary>
        public Damageable Target
        {
            get => _target; set
            {
                _target = value;
                lastHp = value.HP;
            }
        }

        /// <summary>
        /// Parameters for the bloodstains effect
        /// </summary>
        [SerializeField] BloodstainsConfig Bloodstains = BloodstainsConfig.Default;


        /// <summary>
        /// Parameters for the bloodstains effect
        /// </summary>
        [System.Serializable]
        public struct BloodstainsConfig
        {
            /// <summary>
            /// Maximum seconds the effect can take (actual time will vary depending on the seriousnes of the injury).
            /// </summary>
            public float MaxBloodstainsDuration;
            /// <summary>
            /// Fixed number of seconds the buildup will take
            /// </summary>
            public float BloodstainsBuildup;
            /// <summary>
            /// Minimum portion of health taken (value in [0;1] interval) to show any bloodstains effect.
            /// </summary>
            public float MinimumHealthLossForBloodstains;

            /// <summary>
            /// Default bloodstains parameters
            /// </summary>
            public static BloodstainsConfig Default => new() { MaxBloodstainsDuration = 5f, BloodstainsBuildup = 0.1f, MinimumHealthLossForBloodstains = 0.2f };
        }

        float lastHp;
        private void Start()
        {
            if (Target.IsNotNil()) lastHp = Target.HP;
        }
        private void Update()
        {
            if (Target.IsNil()) return;
            var deltaHp = Target.HP - lastHp;
            UpdateHealthbar(Target);
            TryShowBloodstains(deltaHp, Target.MaxHP);
            lastHp = Target.HP;
        }

        void UpdateHealthbar(Damageable target)
        {
            HealthBar.Value = target.HP / target.MaxHP;
        }

        void TryShowBloodstains(float deltaHp, float maxHp)
        {
            float healthLossRatio = -deltaHp / maxHp;
            if (healthLossRatio >= Bloodstains.MinimumHealthLossForBloodstains)
            {
                BloodstainsInstancer.Show(Bloodstains.MaxBloodstainsDuration, buildupTime:Mathf.Min( Bloodstains.BloodstainsBuildup, Bloodstains.MaxBloodstainsDuration));
            }
        }

    }
}