using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.PhysicsUtils
{
    /// <summary>
    /// Collider layers defined in the project. To not waste hours of debugging with Unity's default intly typed nonsense.
    /// </summary>
    public static class ColliderLayer
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

        static int init([System.Runtime.CompilerServices.CallerMemberName] string fieldName = null) => LayerMask.NameToLayer(fieldName);
    }

}