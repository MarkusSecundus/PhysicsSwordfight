using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.Utils.Datastructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    /// <summary>
    /// Component that connects joints of its gameobject to the first rigidbody it collides with and then destroys itself.
    /// </summary>
    [RequireComponent(typeof(Joint))]
    public class AutoattachJointToFirstCollision : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            var rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb == null) return;

            var joints = GetComponents<Joint>();
            if (joints.IsNullOrEmpty()) throw new System.ArgumentException($"Object {gameObject.name} has no component of type {nameof(Joint)}!");

            foreach(var joint in joints)
                if(joint.connectedArticulationBody.IsNil()) joint.connectedBody ??= rb;

            Destroy(this);
        }
    }
}