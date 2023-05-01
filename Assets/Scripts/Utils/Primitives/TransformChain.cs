using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    /// <summary>
    /// Lightweight object that represents chain of transforms that are linked through ancestor-descendant relation in transform hierarchy.
    /// </summary>
    [System.Serializable]
    public struct TransformChain
    {
        /// <summary>
        /// Root gameobject of the chain
        /// </summary>
        public Transform Root;
        /// <summary>
        /// Leaf gameobject of the chain
        /// </summary>
        public Transform Tip;

        /// <summary>
        /// Checks if root and tip are related to each other in the correct way
        /// </summary>
        /// <returns><c>true</c> IFF <see cref="Tip"/> is descendant of <see cref="Root"/></returns>
        public bool IsValid() => Root != null && Tip != null && Tip.IsDescendantOf(Root);

        /// <summary>
        /// Creates an array containing all the transforms in the chain
        /// </summary>
        /// <returns>Array with all the transforms in the chain</returns>
        public Transform[] ToArray() => UnityEngine.Animations.Rigging.ConstraintsUtils.ExtractChain(Root, Tip);
    }
}
