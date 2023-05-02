using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Sword.AI
{
    /// <summary>
    /// States which the <see cref="SwordsmanAI"/> can find itself in, that determine what moves he does with his sword.
    /// </summary>
    [System.Serializable]
    public enum SwordRecordUsecase
    {
        /// <summary>
        /// Swordsman is attacking
        /// </summary>
        Attack, 
        /// <summary>
        /// Swordsman is idle
        /// </summary>
        Idle, 
        /// <summary>
        /// Swordsman is blocking enemy's attack
        /// </summary>
        Block
    }
}