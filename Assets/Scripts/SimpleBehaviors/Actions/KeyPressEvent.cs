using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    public class KeyPressEvent : MonoBehaviour
    {
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