using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Animation
{
    /// <summary>
    /// Component to ensure swordsman's detailed body is holding sword in its hands.
    /// </summary>
    public class SwordsmanBodyProceduralAnimation : MonoBehaviour
    {
        /// <summary>
        /// Sword to hold
        /// </summary>
        [Tooltip("Sword to hold")]
        public SwordDescriptor Sword;

        /// <summary>
        /// Description of the bone rig to be procedurally animated
        /// </summary>
        [Tooltip("Description of the bone rig to be procedurally animated")]
        [SerializeField] private SwordsmanModelDescriptor Model;

        /// <summary>
        /// Description of the bone rig to be procedurally animated
        /// </summary>
        [Tooltip("Description of the bone rig to be procedurally animated")]
        [System.Serializable]
        public struct SwordsmanModelDescriptor
        {
            /// <summary>
            /// Rig bones for swordsman's right arm
            /// </summary>
            [Tooltip("Rig bones for swordsman's right arm")]
            public ArmIKDescriptor RightArm;
            /// <summary>
            /// Rig bones for swordsman's left arm
            /// </summary>
            [Tooltip("Rig bones for swordsman's left arm")]
            public ArmIKDescriptor LeftArm;

            [System.Serializable]
            public struct ArmIKDescriptor
            {
                /// <summary>
                /// Point defining IK target
                /// </summary>
                [Tooltip("Point defining IK target")]
                public Transform Target;
                /// <summary>
                /// Point defining IK hint
                /// </summary>
                [Tooltip("Point defining IK hint")]
                public Transform Hints;
                /// <summary>
                /// Point where the hand should be looking at
                /// </summary>
                [Tooltip("Point where the hand should be looking at")]
                public Transform Look;
                /// <summary>
                /// Bone chain describing the hand
                /// </summary>
                [Tooltip("Bone chain describing the hand")]
                public Transform[] Bones;
                /// <summary>
                /// Leaf hand bone
                /// </summary>
                [Tooltip("Leaf hand bone")]
                public Transform HandTipBone;
            }

            public float ArmElasticity;
        }
        SwordsmanBodyProceduralAnimation()
        {
            Model.ArmElasticity = 0.8f;
        }


        // Update is called once per frame
        void Update()
        {
            SetupArm(Model.RightArm, Sword.SwordHandleUpHandTarget);
            SetupArm(Model.LeftArm, Sword.SwordHandleDownHandTarget);
        }

        private void SetupArm(SwordsmanModelDescriptor.ArmIKDescriptor arm, Transform target)
        {
            var rootBone = arm.Bones[0];
            var armLength = (float)arm.Bones.ComputeChainLength();
            var handLength = arm.HandTipBone.position.Distance(arm.HandTipBone.parent.position);
            var targetDirection = (target.position - rootBone.position).Normalized(out var targetDistance);
            var holdDistance = Mathf.Min(targetDistance - handLength / 2, Model.ArmElasticity * armLength);

            arm.Target.position = rootBone.position + holdDistance * targetDirection;
            arm.Look.position = rootBone.position + 2 * holdDistance * targetDirection;
        }
    }
}