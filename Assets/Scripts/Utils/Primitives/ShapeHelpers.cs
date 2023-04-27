using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    public static class ShapeHelpers
    {
        public static Rect RectFromPoints(Vector2 a, Vector2 b)
        {
            if (a.x > b.x) (a.x, b.x) = (b.x, a.x);
            if (a.y > b.y) (a.y, b.y) = (b.y, a.y);
            return new Rect(a, b - a);
        }
        public static void SetRect(this RectTransform self, Rect toSet)
        {
            self.sizeDelta = toSet.size;
            self.position = toSet.center;
        }
        public static Rect GetRect(this RectTransform self)
        {
            if (self.parent == null) return self.rect;
            var (min, max) = (self.LocalToGlobal(self.rect.min), self.LocalToGlobal(self.rect.max));
            return RectFromPoints(min, max);
        }

        public static Rect AddSize(this Rect self, Vector2 toAdd)
            => new Rect(self.min, self.size + toAdd);
        public static Rect AddPosition(this Rect self, Vector2 toAdd)
            => new Rect(self.min + toAdd, self.size);

    }
}
