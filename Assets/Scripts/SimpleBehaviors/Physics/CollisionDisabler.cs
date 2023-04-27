using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    public class CollisionDisabler : MonoBehaviour
    {
        public Collider[] ToDisable;


        // Start is called before the first frame update
        void Start()
        {
            var localColliders = GetComponentsInChildren<Collider>();
            foreach (var local in localColliders)
                foreach (var other in ToDisable)
                    Physics.IgnoreCollision(local, other);
        }
    }
}