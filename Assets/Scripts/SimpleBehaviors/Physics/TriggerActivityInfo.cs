using MarkusSecundus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    /// <summary>
    /// Component for keeping track of ongoing collisions of a triggercollider.
    /// 
    /// <para>
    /// Must be inside the same gameobject as triggercolliders or in the root of a rigidbody that has some triggercolliders inside it.
    /// </para>
    /// </summary>
    public class TriggerActivityInfo : MonoBehaviour
    {
        /// <summary>
        /// How many triggercolliders are currently in collision with <c>this</c> triggercollider
        /// </summary>
        public int ActiveTriggerCollidersCount => activeTriggerCollidersCount;
        /// <summary>
        /// How many non-trigger colliders are currently in collision with <c>this</c> triggercollider
        /// </summary>
        public int ActiveNormalCollidersCount => activeNormalCollidersCount;
        /// <summary>
        /// List of all colliders that are currently colliding with <c>this</c> triggercollider.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<Collider> GetActiveTriggers() => activeTriggers.Keys;




        private Dictionary<Collider, int> activeTriggers = new Dictionary<Collider, int>();

        private int activeTriggerCollidersCount = 0, activeNormalCollidersCount = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (activeTriggers.TryGetValue(other, out var count)) activeTriggers[other] = ++count;
            else
            {
                activeTriggers[other] = 1;
                ++(other.isTrigger ? ref activeTriggerCollidersCount : ref activeNormalCollidersCount);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!activeTriggers.TryGetValue(other, out var count) || count < 1)
                Debug.LogError($"This shouldn't happen - exiting collider {other.name} which wasn't present in the active trigger set of {this.name}");
            if (--count < 1)
            {
                activeTriggers.Remove(other);
                --(other.isTrigger ? ref activeTriggerCollidersCount : ref activeNormalCollidersCount);
            }
            else
                activeTriggers[other] = count;
        }
    }
}