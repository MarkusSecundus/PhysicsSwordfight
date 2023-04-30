using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    /// <summary>
    /// Simple action that listens for keypresses and fires events registered for particular keys
    /// </summary>
    public class KeyPressEvent : MonoBehaviour
    {
        /// <summary>
        /// Map of events to be invoked for specific keys being pressed
        /// </summary>
        public SerializableDictionary<KeyCode, UnityEvent> Events;

        void Update()
        {
            if (UnityEngine.Input.anyKeyDown)
            {
                foreach (var (key, @event) in Events.Values)
                {
                    if (UnityEngine.Input.GetKeyDown(key))
                        @event.Invoke();
                }
            }
        }
    }
}