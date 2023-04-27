using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    public static class PhysicsHelpers
    {

        public static IEnumerable<ContactPoint> IterateContacts(this Collision self)
        {
            for (int t = 0; t < self.contactCount; ++t) yield return self.GetContact(t);
        }


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

        public static void MoveToVelocity(this Rigidbody self, Vector3 velocity)
        {
            var toApply = velocity - self.velocity;
            self.AddForce(toApply, ForceMode.VelocityChange);
        }
        public static void MoveToAngularVelocity(this Rigidbody self, Vector3 velocity)
        {
            var toApply = velocity - self.angularVelocity;
            self.AddTorque(toApply, ForceMode.VelocityChange);
            //Debug.Log($"applied torque {toApply}, target: {velocity} -> velocity{self.angularVelocity}");
        }

        public static Rigidbody MimicVolatilesOf(this Rigidbody self, Rigidbody toMimic)
        {
            self.position = toMimic.position;
            self.rotation = toMimic.rotation;
            self.velocity = toMimic.velocity;
            self.angularVelocity = toMimic.angularVelocity;

            return self;
        }

        public static Rigidbody MimicStateOf(this Rigidbody self, Rigidbody toMimic)
        {

            self.automaticCenterOfMass = false;
            self.centerOfMass = toMimic.centerOfMass;

            self.automaticInertiaTensor = false;
            self.inertiaTensor = toMimic.inertiaTensor;
            self.inertiaTensorRotation = toMimic.inertiaTensorRotation;

            self.drag = toMimic.drag;
            self.angularDrag = toMimic.angularDrag;
            self.maxAngularVelocity = toMimic.maxAngularVelocity;
            self.maxDepenetrationVelocity = toMimic.maxDepenetrationVelocity;
            self.maxLinearVelocity = toMimic.maxLinearVelocity;
            self.useGravity = toMimic.useGravity;
            self.mass = toMimic.mass;
            self.maxDepenetrationVelocity = toMimic.maxDepenetrationVelocity;

            self.sleepThreshold = toMimic.sleepThreshold;
            self.solverIterations = toMimic.solverIterations;
            self.solverVelocityIterations = toMimic.solverVelocityIterations;

            return self.MimicVolatilesOf(toMimic);
        }

        public static Rigidbody MimicAllOf(this Rigidbody self, Rigidbody toMimic)
        {
            self.collisionDetectionMode = toMimic.collisionDetectionMode;
            self.constraints = toMimic.constraints;
            self.detectCollisions = toMimic.detectCollisions;
            self.excludeLayers = toMimic.excludeLayers;
            self.freezeRotation = toMimic.freezeRotation;
            self.includeLayers = toMimic.includeLayers;
            self.interpolation = toMimic.interpolation;
            self.isKinematic = toMimic.isKinematic;

            return self.MimicStateOf(toMimic);
        }
    }

}
