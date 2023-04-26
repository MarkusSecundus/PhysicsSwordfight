using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Animation
{
    public class SwordsmanBodyProceduralAnimation : MonoBehaviour
    {
        public SwordDescriptor Sword;

        [SerializeField] private SwordsmanModelDescriptor Model;

        [System.Serializable]
        public struct SwordsmanModelDescriptor
        {
            public ArmIKDescriptor RightArm, LeftArm;

            [System.Serializable]
            public struct ArmIKDescriptor
            {
                public Transform Target, Hints, Look;
                public Transform[] Bones;
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