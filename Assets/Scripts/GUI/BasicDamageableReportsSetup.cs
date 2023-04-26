using MarkusSecundus.PhysicsSwordfight.GUI;
using MarkusSecundus.PhysicsSwordfight.Sword.Damage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    [RequireComponent(typeof(Damageable))]
    public class BasicDamageableReportsSetup : MonoBehaviour
    {
        private void Start() => InitializeReports();

        public void InitializeReports()
        {
            var damageable = GetComponent<Damageable>();
            var reporter = DamageReporter.Get();
            damageable.OnDamaged.AddListener(reporter.ReportDamage);
            damageable.OnHealed.AddListener(reporter.ReportHeal);
            damageable.OnDeath.AddListener(reporter.ReportDeath);
        }
    }
}