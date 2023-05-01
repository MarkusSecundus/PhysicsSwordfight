using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    /// <summary>
    /// Static class containing convenience extensions methods for primitive shape types.
    /// </summary>
    public static class ShapeHelpers
    {
        /// <summary>
        /// Constuct rectangle from two edge points.
        /// </summary>
        /// <param name="a">One edge point</param>
        /// <param name="b">Other edge point</param>
        /// <returns>Rectangle with the provided edge points</returns>
        public static Rect RectFromPoints(Vector2 a, Vector2 b)
        {
            if (a.x > b.x) (a.x, b.x) = (b.x, a.x);
            if (a.y > b.y) (a.y, b.y) = (b.y, a.y);
            return new Rect(a, b - a);
        }

        /// <summary>
        /// Get rectangle in worldspace describing given <see cref="RectTransform"/>
        /// </summary>
        /// <param name="self"><c>this</c></param>
        /// <returns>Rectangle of <paramref name="self"/> in worldspace </returns>
        public static Rect GetRect(this RectTransform self)
        {
            if (self.parent == null) return self.rect;
            var (min, max) = (self.LocalToGlobal(self.rect.min), self.LocalToGlobal(self.rect.max));
            return RectFromPoints(min, max);
        }
    }
}
