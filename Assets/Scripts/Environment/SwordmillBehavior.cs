using DG.Tweening;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Environment
{
    /// <summary>
    /// Component responsible for the movement of Swordmill's rotor
    /// </summary>
    [RequireComponent(typeof(SwordmillAssembly))]
    public class SwordmillBehavior : MonoBehaviour
    {
        /// <summary>
        /// Describes one step of the swordmill rotor's movement path
        /// </summary>
        [System.Serializable]
        public struct MovementStep
        {
            /// <summary>
            /// Target position - point relative to the original rotation of swordmill's rotor
            /// </summary>
            public Vector3 Offset;
            /// <summary>
            /// How many seconds traveling to this position will take
            /// </summary>
            public float Duration;
        }
        /// <summary>
        /// Describes one step of the swordmill rotor's rotation path
        /// </summary>
        [System.Serializable]
        public struct RotationStep
        {
            /// <summary>
            /// Target rotation of the rotor
            /// </summary>
            public Vector3 Rotation;
            /// <summary>
            /// How many seconds getting to this position will take
            /// </summary>
            public float Duration;
        }
        /// <summary>
        /// List of steps describing the swordmill's movement cycle
        /// </summary>
        public MovementStep[] Movements;
        /// <summary>
        /// List of steps describing the swordmill's rotation cycle
        /// </summary>
        public RotationStep[] Rotations;


        SwordmillAssembly assembly;
        ConfigurableJoint rotor => assembly.Rotor;

        Vector3 originalConnectedAnchor, originalTargetAngularVelocity;
        void Start()
        {
            assembly = GetComponent<SwordmillAssembly>();
            originalConnectedAnchor = rotor.connectedAnchor;
            rotor.autoConfigureConnectedAnchor = false;
            rotor.connectedAnchor = originalConnectedAnchor;
            originalTargetAngularVelocity = rotor.targetAngularVelocity;
            UpdateMovement(0);
            AnimateRotation(0);
        }

        void UpdateMovement(int index)
        {
            if (Movements.Length <= 0) return;

            var order = Movements[index %= Movements.Length];
            rotor.DOConnectedAnchor(originalConnectedAnchor + order.Offset, order.Duration)
                .OnComplete(() => UpdateMovement(index + 1));
        }

        void AnimateRotation(int index)
        {
            if (Rotations.Length <= 0) return;

            var order = Rotations[index %= Rotations.Length];
            rotor.DOTargetAngularVelocity(originalTargetAngularVelocity + order.Rotation, order.Duration)
                .OnComplete(() => AnimateRotation(index + 1));
        }
    }
}