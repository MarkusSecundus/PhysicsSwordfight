using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyPressEvent : MonoBehaviour
{
    public SerializableDictionary<KeyCode, UnityEvent> Events;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach(var (key, @event) in Events.Values)
            {
                if (Input.GetKeyDown(key))
                    @event.Invoke();
            }
        }
    }
}
