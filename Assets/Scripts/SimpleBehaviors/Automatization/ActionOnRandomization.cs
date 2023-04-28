using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Automatization
{

    public class ActionOnRandomization : MonoBehaviour, IRandomizer
    {
        public UnityEvent<System.Random> ToInvoke;

        public void Randomize(System.Random random) => ToInvoke.Invoke(random);
    }
}