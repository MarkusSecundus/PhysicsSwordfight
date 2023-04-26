using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Utils.Constants
{
    public static class ColliderLayers
    {
        public static readonly int
            Default = init(),
            TransparentFX = init(),
            IgnoreRaycast = init(),
            Water = init(),
            UI = init(),
            FloorLayer = init(),
            ActiveRagdollParts = init(),
            NoCollisions = init(),
            Swords = init(),
            PlayerDetailedBody = init(),
            PlayerSimpleBody = init(),
            CollisionFix = init();

        private static int init([System.Runtime.CompilerServices.CallerMemberName] string fieldName = null) => LayerMask.NameToLayer(fieldName);
    }

}