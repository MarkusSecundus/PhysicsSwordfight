using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    [System.Serializable]
    public class JointRotationHelper
    {
        public ConfigurableJoint Joint { get; }
        public Space Space { get; }

        [SerializeField]
        internal Quaternion startRotation;
        public JointRotationHelper(ConfigurableJoint joint)
        {
            this.Space = joint.configuredInWorldSpace ? Space.World : Space.Self;
            this.Joint = joint;
            startRotation = Space switch
            {
                Space.Self => joint.transform.localRotation,
                Space.World => joint.transform.rotation,
                _ => throw new ArgumentException($"Invalid value `{Space}` provided for {nameof(Space)}")
            };
            CurrentRotation = Quaternion.identity;
        }

        public Quaternion CurrentRotation { get; private set; }

        public void SetTargetRotation(Quaternion newTargetRotation)
            => JointExtensions.SetTargetRotationInternal(Joint, CurrentRotation = newTargetRotation, startRotation, Space);
    }
}
