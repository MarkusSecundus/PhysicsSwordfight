using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    /// <summary>
    /// Static class containing convenience extensions methods for physics-related stuff
    /// </summary>
    public static class PhysicsHelpers
    {
        /// <summary>
        /// Iterate through contacts provided by <see cref="Collision"/> without leaking memory.
        /// </summary>
        /// <param name="self">Collision that's currently being resolved</param>
        /// <returns>Generator that iterates through collission contacts</returns>
        public static IEnumerable<ContactPoint> IterateContacts(this Collision self)
        {
            for (int t = 0; t < self.contactCount; ++t) yield return self.GetContact(t);
        }

        /// <summary>
        /// Try to obtain the collider that resides in the gameobject that is currently receiving OnCollision callback.
        /// 
        /// <para>
        /// Will fail and return <c>null</c> if the collision has no contacts, because Unity devs are f**ing i****s who can't just add this field directly to the <see cref="Collision"/>.
        /// </para>
        /// </summary>
        /// <param name="self">Collision that's currently being resolved</param>
        /// <returns>thisCollider or <c>null</c> if there are no contact points</returns>
        public static Collider ThisCollider(this Collision self)
        {
            if (self.contactCount <= 0)
            {
                Debug.Log($"No contacts - total force was: {self.impulse.ToStringPrecise()} relVel{self.relativeVelocity.ToStringPrecise()}");
                return null;// throw new System.InvalidOperationException($"This collision doesn't have any contacts! ({self.gameObject.name})");

            }
            var contact = self.GetContact(0);
            var ret = contact.thisCollider;
            if (ret == self.collider)
            {
                ret = contact.otherCollider;
                Debug.Log("Had to try realy hard!", ret);
            }
            if (ret == self.collider) return null;// throw new System.InvalidOperationException($"All the colliders of this collision are the same one!");
            return ret;
        }

        /// <summary>
        /// Apply force to a rigidbody exactly enough to set it to desired velocity.
        /// </summary>
        /// <param name="self">Rigidbody to apply force on</param>
        /// <param name="velocity">Target velocity</param>
        public static void MoveToVelocity(this Rigidbody self, Vector3 velocity)
        {
            var toApply = velocity - self.velocity;
            self.AddForce(toApply, ForceMode.VelocityChange);
        }
        /// <summary>
        /// Apply torque to a rigidbody exactly enough to set it to desired velocity.
        /// </summary>
        /// <param name="self">Rigidbody to apply torque on</param>
        /// <param name="velocity">Target angular velocity</param>
        public static void MoveToAngularVelocity(this Rigidbody self, Vector3 velocity)
        {
            var toApply = velocity - self.angularVelocity;
            self.AddTorque(toApply, ForceMode.VelocityChange);
        }
    }

}
