using MarkusSecundus.PhysicsSwordfight.Cosmetics;
using MarkusSecundus.PhysicsSwordfight.GUI;
using MarkusSecundus.PhysicsSwordfight.Sword;
using MarkusSecundus.PhysicsSwordfight.Sword.Damage;
using MarkusSecundus.PhysicsSwordfight.Symbols;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword
{
    /// <summary>
    /// Component responsible for linking the sword and swordsman together.
    /// </summary>
    public class SwordsmanAssembly : MonoBehaviour
    {
        /// <summary>
        /// Swordsman instance
        /// </summary>
        public SwordsmanMovement Player;
        /// <summary>
        /// Sword instance
        /// </summary>
        public SwordMovement Sword;
        /// <summary>
        /// Actual camera that should follow the camera stub inside the Swordsman. Optional parameter.
        /// </summary>
        public ComponentSymbol<CameraFollowPoint> Camera;
        /// <summary>
        /// HUD to be used for displaying swordsman's HP state
        /// </summary>
        public ComponentSymbol<DamageHUD> DamageReport;

        /// <summary>
        /// Parameters for swordsman's death actions
        /// </summary>
        [System.Serializable]
        public struct InstructionsOnDeath
        {
            /// <summary>
            /// How many seconds after the swordsman's death should the sword get destroyed as well
            /// </summary>
            public float SwordDestroyDelay;
        }
        /// <summary>
        /// Parameters for swordsman's death actions
        /// </summary>
        public InstructionsOnDeath DeathInstructions;

        void Start()
        {
            var camera = Camera.Get();
            if (camera.IsNotNil() && Player.CameraToUse.IsNotNil())
            {
                camera.Target = Player.CameraToUse;
            }
            var damageReport = DamageReport.Get();
            if (damageReport.IsNotNil())
            {
                damageReport.Target = Player.GetComponent<Damageable>();
            }
        }
        /// <summary>
        /// Drops the sword and destroys the swordsman in controlled way
        /// </summary>
        public void DestroyTheSwordsman()
        {
            if (DeathInstructions.SwordDestroyDelay.IsNormalNumber() && DeathInstructions.SwordDestroyDelay >= -0f) Destroy(Sword.gameObject, DeathInstructions.SwordDestroyDelay);
            Sword.DropTheSword();
            Destroy(gameObject);
        }
    }
}