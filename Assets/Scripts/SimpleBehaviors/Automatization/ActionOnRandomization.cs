using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Automatization
{
    /// <summary>
    /// Custom callback to be called on object's randomization event
    /// </summary>
    public class ActionOnRandomization : MonoBehaviour, IRandomizer
    {
        /// <summary>
        /// Event to be ran on randomization
        /// </summary>
        public UnityEvent<System.Random> ToInvoke;

        /// <inheritdoc/>
        public void Randomize(System.Random random) => ToInvoke.Invoke(random);
    }
}