using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    public class EventTimer : MonoBehaviour
    {
        public UnityEvent Event;

        public float DelaySeconds = 1f;

        public bool OnStartup = false;

        // Start is called before the first frame update
        void Start()
        {
            if (OnStartup) StartTimer();
        }


        void StartTimer()
        {
            this.PerformWithDelay(Event.Invoke, DelaySeconds);
        }
    }
}