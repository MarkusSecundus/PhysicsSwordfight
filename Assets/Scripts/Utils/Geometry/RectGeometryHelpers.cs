using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Geometry
{
    /// <summary>
    /// Static class providing methods for performing geometric computations on rectangles
    /// </summary>
    public static class RectGeometryHelpers
    {
        /// <summary>
        /// Compute rectangle describing set of all points where placing a bigger rect covers the whole of a smaller rect.
        /// </summary>
        /// <param name="biggerOne">Rect that is in all dimensions greater than <paramref name="smallerRect"/></param>
        /// <param name="smallerRect">Rect that is in all dimensions smaller than <paramref name="biggerOne"/></param>
        /// <returns></returns>
        public static Rect PositionsWherePlacingThisRectCoversTheWholeOfSmallerRect(this Rect biggerOne, Rect smallerRect)
        {
            Vector2 min = smallerRect.max - biggerOne.size / 2f, max = smallerRect.min + biggerOne.size / 2f;
            return ShapeHelpers.RectFromPoints(min, max);
        }
    }
}
