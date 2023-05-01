using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.Utils.Datastructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword
{
    /// <summary>
    /// List of points of interest that describe a sword.
    /// </summary>
    public class SwordDescriptor : MonoBehaviour
    {
        /// <summary>
        /// Point in which the sword is held by the swordsman.
        /// </summary>
        public Transform SwordAnchor => anchor = anchor.IfNil(target?.SwordAnchor);
        /// <summary>
        /// Sword's centre of mass.
        /// </summary>
        public Transform SwordCenterOfMass => centerOfWeight = centerOfWeight.IfNil(target?.SwordCenterOfMass);
        /// <summary>
        /// Tip of the blade - upmost point of the sword.
        /// </summary>
        public Transform SwordTip => tipPoint = tipPoint.IfNil(target?.SwordTip);

        /// <summary>
        /// Point in the upper part of the handle where the swordsman should hold the sword (with his dominant hand).
        /// </summary>
        public Transform SwordHandleUpHandTarget => upHandTarget = upHandTarget.IfNil(target?.SwordHandleUpHandTarget);
        /// <summary>
        /// Point in the lower part of the handle where the swordsman should hold the sword (with his non-dominant hand).
        /// </summary>
        public Transform SwordHandleDownHandTarget => downHandTarget = downHandTarget.IfNil(target?.SwordHandleDownHandTarget);
        /// <summary>
        /// Point on the blade's edge near the handle, which the swordsman will try to position to catch incoming strikes.
        /// </summary>
        public Transform SwordBlockPoint => blockPoint = blockPoint.IfNil(target?.SwordBlockPoint);
        /// <summary>
        /// Bottom of the sword's handle - downmost point of the sword.
        /// </summary>
        public Transform SwordBottom => bottom = bottom.IfNil(target?.SwordBottom);

        [SerializeField] private SwordDescriptor target = null;
        [SerializeField] private Transform anchor = null, centerOfWeight = null, tipPoint = null, blockPoint = null, upHandTarget = null, downHandTarget = null, bottom = null;
    }

    /// <summary>
    /// Static class containing convenient extension methods for <see cref="SwordMovement"/>.
    /// </summary>
    public static class SwordDescriptorExtensions
    {
        /// <summary>
        /// Gets ray going along the sword's axis from its bottom to the tip, in worldspace.
        /// </summary>
        /// <param name="self"><c>this</c> descriptor instance</param>
        /// <returns>Ray going along the sword's axis from its bottom to the tip, in worldspace</returns>
        public static ScaledRay SwordAsRay(this SwordDescriptor self)
        {
            var botom = self.SwordBottom.position;
            var direction = self.SwordTip.position - botom;
            return ScaledRay.FromDirection(botom, direction);
        }

        /// <summary>
        /// Gets ray going along the sword's blade's axis from where the blade starts to its tip, in worldspace.
        /// </summary>
        /// <param name="self"><c>this</c> descriptor instance</param>
        /// <returns>Ray going along the sword's blade's axis from where the blade starts to its tip, in worldspace</returns>
        public static ScaledRay SwordBladeAsRay(this SwordDescriptor self)
        {
            var botom = self.SwordAnchor.position;
            var direction = self.SwordTip.position - botom;
            return ScaledRay.FromDirection(botom, direction);
        }

    }
}