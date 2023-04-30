using MarkusSecundus.PhysicsSwordfight.GUI;
using MarkusSecundus.PhysicsSwordfight.Sword.Damage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// Automatically setups callbacks an a <see cref="Damageable"/> instance for textual reporting of damage, healed and death events via the <see cref="DamageReporter"/> singleton.
    /// 
    /// <para>
    /// Requires <see cref="Damageable"/> component to be present on the same game object.
    /// </para>
    /// </summary>
    [RequireComponent(typeof(Damageable))]
    public class BasicDamageableReportsSetup : MonoBehaviour
    {
        private void Start() => InitializeReports();

        /// <summary>
        /// Finds a <see cref="DamageReporter"/> singleton in the scene and setups it to report damaged, healed and death events that occured on this <see cref="Damageable"/>
        /// </summary>
        public void InitializeReports()
        {
            var damageable = GetComponent<Damageable>();
            var reporter = DamageReporter.Get(gameObject);
            damageable.OnDamaged.AddListener(reporter.ReportDamage);
            damageable.OnHealed.AddListener(reporter.ReportHeal);
            damageable.OnDeath.AddListener(reporter.ReportDeath);
        }
    }
}