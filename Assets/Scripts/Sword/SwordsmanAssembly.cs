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
    [Tooltip("Component responsible for linking the sword and swordsman together")]
    public class SwordsmanAssembly : MonoBehaviour
    {
        /// <summary>
        /// Swordsman instance
        /// </summary>
        [Tooltip("Swordsman instance")]
        public SwordsmanMovement Player;
        /// <summary>
        /// Sword instance
        /// </summary>
        [Tooltip("Sword instance")]
        public SwordMovement Sword;
        /// <summary>
        /// Actual camera that should follow the camera stub inside the Swordsman. Optional parameter.
        /// </summary>
        [Tooltip("Actual camera that should follow the camera stub inside the Swordsman. Optional parameter.")]
        public ComponentSymbol<CameraFollowPoint> Camera;
        /// <summary>
        /// HUD to be used for displaying swordsman's HP state
        /// </summary>
        [Tooltip("HUD to be used for displaying swordsman's HP state")]
        public ComponentSymbol<DamageHUD> DamageReport;

        /// <summary>
        /// Parameters for swordsman's death actions
        /// </summary>
        [Tooltip("Parameters for swordsman's death actions")]
        [System.Serializable]
        public struct InstructionsOnDeath
        {
            /// <summary>
            /// How many seconds after the swordsman's death should the sword get destroyed as well
            /// </summary>
            [Tooltip("How many seconds after the swordsman's death should the sword get destroyed as well")]
            public float SwordDestroyDelay;
        }
        /// <summary>
        /// Parameters for swordsman's death actions
        /// </summary>
        [Tooltip("Parameters for swordsman's death actions")]
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
            try
            {
                if (Sword) Sword.DropTheSword();
                if (DeathInstructions.SwordDestroyDelay.IsNormalNumber() && DeathInstructions.SwordDestroyDelay >= -0f) Destroy(Sword.gameObject, DeathInstructions.SwordDestroyDelay);
            }
            finally
            {
                Destroy(gameObject);
            }
        }
    }
}