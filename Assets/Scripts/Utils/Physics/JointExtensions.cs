using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    /// <summary>
    /// All credit goes to mstevenson <see href="https://gist.github.com/mstevenson/4958837"/>
    /// </summary>
    public static class JointExtensions
    {
        public static JointRotationHelper MakeRotationHelper(this ConfigurableJoint self) => new JointRotationHelper(self);
        /// <summary>
        /// Sets a joint's targetRotation to match a given local rotation.
        /// The joint transform's local rotation must be cached on Start and passed into this method.
        /// 
        /// <para>All credit goes to mstevenson <see href="https://gist.github.com/mstevenson/4958837"/></para>
        /// </summary>
        /// <param name="joint">Joint that's being rotated</param>
        /// <param name="targetLocalRotation">Rotation in local space</param>
        /// <param name="startLocalRotation">Rotation the joint had on Start</param>
        public static void SetTargetRotationLocal(this ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
        {
            if (joint.configuredInWorldSpace)
            {
                Debug.LogError("SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", joint);
            }
            SetTargetRotation(joint, targetLocalRotation, startLocalRotation, Space.Self);
        }

        /// <summary>
        /// Sets a joint's targetRotation to match a given world rotation.
        /// The joint transform's world rotation must be cached on Start and passed into this method.
        /// 
        /// <para>All credit goes to mstevenson <see href="https://gist.github.com/mstevenson/4958837"/></para>
        /// </summary>
        /// <param name="joint">Joint that's being rotated</param>
        /// <param name="targetWorldRotation">Rotation in world space</param>
        /// <param name="startWorldRotation">Rotation the joint had on Start</param>
        public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion startWorldRotation)
        {
            if (!joint.configuredInWorldSpace)
            {
                Debug.LogError("SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", joint);
            }
            SetTargetRotation(joint, targetWorldRotation, startWorldRotation, Space.World);
        }
        /// <summary>
        /// Compute value that needs to be set to a joint's targetRotation to match a given real rotation.
        /// The joint transform's real rotation must be cached on Start and passed into this method.
        /// 
        /// <para>All credit goes to mstevenson <see href="https://gist.github.com/mstevenson/4958837"/></para>
        /// </summary>
        /// <param name="joint">Joint that's being rotated</param>
        /// <param name="targetRotation">Rotation in local or world space</param>
        /// <param name="startRotation">Rotation the joint had on Start</param>
        /// <param name="space">world space or local space</param>
        /// <returns>Quaternion to set to joint's target rotation</returns>
        public static Quaternion ComputeTargetRotation(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
        {
            // Calculate the rotation expressed by the joint's axis and secondary axis
            var right = joint.axis;
            var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
            var up = Vector3.Cross(forward, right).normalized;
            Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

            // Transform into world space
            Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

            // Counter-rotate and apply the new local rotation.
            // Joint space is the inverse of world space, so we need to invert our value
            if (space == Space.World)
            {
                resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
            }
            else
            {
                resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
            }

            // Transform back into joint space
            resultRotation *= worldToJointSpace;

            // Return our newly calculated rotation
            return resultRotation;
        }
        /// <summary>
        /// Sets a joint's targetRotation to match a given world or local rotation.
        /// The joint transform's rotation must be cached on Start and passed into this method.
        /// 
        /// <para>All credit goes to mstevenson <see href="https://gist.github.com/mstevenson/4958837"/></para>
        /// </summary>
        /// <param name="joint">Joint that's being rotated</param>
        /// <param name="targetRotation">Rotation in local or world space</param>
        /// <param name="startRotation">Rotation the joint had on Start</param>
        /// <param name="space">world space or local space</param>
        public static void SetTargetRotation(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
        {
            // Set target rotation to our newly calculated rotation
            joint.targetRotation = ComputeTargetRotation(joint, targetRotation, startRotation, space);
        }
    }
}
