using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    /// <summary>
    /// Declaration of an attack attempt. To serve as argument for <see cref="IArmorPiece.ProcessAttack(AttackDeclaration)"/>.
    /// </summary>
    [System.Serializable]
    public struct AttackDeclaration
    {
        /// <summary>
        /// Object describing the weapon's point of impact
        /// </summary>
        [System.Serializable]
        public struct ImpactPointData
        {
            /// <summary>
            /// Point at which the weapon impacted the <see cref="IArmorPiece"/>
            /// </summary>
            [field: SerializeField] public Vector3 Point { get; set; }
            /// <summary>
            /// Direction with which the weapon impacted the <see cref="IArmorPiece"/>
            /// </summary>
            [field: SerializeField] public Vector3 Normal { get; set; }
        }
        /// <summary>
        /// Ammount of damage to be caused
        /// </summary>
        [field: SerializeField] public float Damage { get; set; }
        /// <summary>
        /// Unique identifier of the attacker
        /// </summary>
        [field: SerializeField] public Object AttackerIdentifier { get; set; }
        /// <summary>
        /// Name of the attacker - for logging purposes etc.
        /// </summary>
        public string AttackerName => AttackerIdentifier.name;
        /// <summary>
        /// Weapon's point of impact
        /// </summary>
        [field: SerializeField] public ImpactPointData ImpactPoint { get; set; }
    }

    /// <summary>
    /// Static class containing convenient extension methods for <see cref="AttackDeclaration"/>.
    /// </summary>
    public static class AttackDeclarationExtensions
    {
        /// <summary>
        /// Generate <see cref="AttackDeclaration.ImpactPoint"/> from a physics collision's impact point
        /// </summary>
        /// <param name="self">Impact point of a physics collision</param>
        /// <returns>Attack point to be used in <see cref="AttackDeclaration"/></returns>
        public static AttackDeclaration.ImpactPointData AsImpactPointData(this ContactPoint self) => new AttackDeclaration.ImpactPointData { Point = self.point, Normal = self.normal };
    }
}