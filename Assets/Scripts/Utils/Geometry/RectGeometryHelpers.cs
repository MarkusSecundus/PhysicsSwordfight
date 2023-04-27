using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    public static class RectGeometryHelpers
    {
        public static Rect PositionsWherePlacingThisRectCoversTheWholeOfSmallerRect(this Rect biggerOne, Rect smallerRect)
        {
            Vector2 min = smallerRect.max - biggerOne.size / 2f, max = smallerRect.min + biggerOne.size / 2f;
            return ShapeHelpers.RectFromPoints(min, max);
        }
    }
}
