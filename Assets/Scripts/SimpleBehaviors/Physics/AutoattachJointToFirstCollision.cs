using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    public class AutoattachJointToFirstCollision : MonoBehaviour
    {

        private void OnCollisionEnter(Collision collision)
        {
            var rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb == null) return;

            var joint = GetComponent<Joint>();
            if (joint == null) throw new System.ArgumentException($"Object {gameObject.name} has no component of type {nameof(Joint)}!");

            joint.connectedBody ??= rb;

            Destroy(this);
        }
    }
}