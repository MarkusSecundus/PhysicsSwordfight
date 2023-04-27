using DG.Tweening;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Environment
{

    [RequireComponent(typeof(SwordmillAssembly))]
    public class SwordmillBehavior : MonoBehaviour
    {
        SwordmillAssembly assembly;

        ConfigurableJoint rotor => assembly.Rotor;
        [System.Serializable]
        public struct MovementStep
        {
            public Vector3 Offset;
            public float Duration;
        }
        [System.Serializable]
        public struct RotationStep
        {
            public Vector3 Rotation;
            public float Duration;
        }

        public MovementStep[] Movements;
        public RotationStep[] Rotations;

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