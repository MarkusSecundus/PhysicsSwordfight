using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    [System.Serializable]
    public struct TransformChain
    {
        public Transform root, tip;

        public bool IsValid() => root != null && tip != null && tip.IsChildOf(root);

        public Transform[] ToArray() => UnityEngine.Animations.Rigging.ConstraintsUtils.ExtractChain(root, tip);
    }
}
