using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    /// <summary>
    /// Wrapper that allows for easy setting rotation on a <see cref="ConfigurableJoint"/>.
    /// 
    /// <para>Based on method formulated by mstevenson <see href="https://gist.github.com/mstevenson/4958837"/></para>
    /// </summary>
    [System.Serializable]
    public class JointRotationHelper
    {
        /// <summary>
        /// Joint whose rotation is being set
        /// </summary>
        public ConfigurableJoint Joint { get; }
        /// <summary>
        /// Whether to use world space or local space
        /// </summary>
        public Space Space { get; }

        [SerializeField]
        internal Quaternion startRotation;
        /// <summary>
        /// Init the rotation helper by given joint.
        /// Must be called from <c>Start</c> handler in the same frame the <see cref="ConfigurableJoint"/> was initialized
        /// </summary>
        /// <param name="joint">Joint to be rotated</param>
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

        /// <summary>
        /// Current rotation of the joint in world or local space
        /// </summary>
        public Quaternion CurrentRotation { get; private set; }

        /// <summary>
        /// Set the joint's target rotation to given world or local value
        /// </summary>
        /// <param name="newTargetRotation">Target rotation in world or local space</param>
        public void SetTargetRotation(Quaternion newTargetRotation)
            => JointExtensions.SetTargetRotation(Joint, CurrentRotation = newTargetRotation, startRotation, Space);
    }
}
