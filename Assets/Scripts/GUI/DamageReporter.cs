using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Args = MarkusSecundus.PhysicsSwordfight.Sword.Damage.Damageable.HpChangedArgs;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// Singleton component responsible for textually logging all damage-related events (entities damaged, healed and killed)
    /// </summary>
    public class DamageReporter : MonoBehaviour
    {
        /// <summary>
        /// Finds the cannonical damage reporter for the provided gameobject - currently always a singleton found by <see cref="Object.FindAnyObjectByType{T}"/>
        /// </summary>
        /// <param name="self">Game object that's searching for its damage reporter.</param>
        /// <returns>Damage reporter to be used by the gameobject.</returns>
        public static DamageReporter Get(GameObject o) => Object.FindAnyObjectByType<DamageReporter>();

        /// <summary>
        /// Report damage
        /// </summary>
        /// <param name="args">Args describing the damage-related event</param>
        public void ReportDamage(Args args)
        {
            Message($"Damaged for {-args.DeltaHP} hp", args.Target);
        }
        /// <summary>
        /// Report heal
        /// </summary>
        /// <param name="args">Args describing the damage-related event</param>
        public void ReportHeal(Args args)
        {
            Message($"Healed for {args.DeltaHP} hp", args.Target);
        }
        /// <summary>
        /// Report death
        /// </summary>
        /// <param name="args">Args describing the damage-related event</param>
        public void ReportDeath(Args args)
        {
            Message("Died!", args.Target);
        }

        private void Message(string s, Object target) => Debug.Log($"{target.name}: {s}", target);
    }
}