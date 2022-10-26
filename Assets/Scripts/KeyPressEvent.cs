using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyPressEvent : MonoBehaviour
{
    public EventInfo[] Events;

    [System.Serializable]
    public struct EventInfo
    {
        public UnityEvent Event;
        public string KeyCode;

        private KeyCode? _code;
        public KeyCode? Code => string.IsNullOrWhiteSpace(KeyCode) ? null : _code ??= (KeyCode?) System.Enum.Parse(typeof(KeyCode), KeyCode);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach(var e in Events)
            {
                if (e.Code == null || Input.GetKeyDown(e.Code.Value))
                    e.Event.Invoke();
            }
        }
    }
}
