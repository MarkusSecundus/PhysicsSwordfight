using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Extensions
{
    public static class TransformHelpers
    {
        public static Vector3 GetPositionRelativeTo(this Transform self, Transform relativeTo)
            => (relativeTo == self) ? Vector3.zero :
               (relativeTo == self.parent) ? self.localPosition :
                  relativeTo.GlobalToLocal(self.position);
        public static Vector3 SetPositionRelativeTo(this Transform self, Transform relativeTo, Vector3 position)
        {
            if (relativeTo == self)
                return (position == Vector3.zero) ? position : throw new System.ArgumentException("Position relative to self cannot be anything other than zero", nameof(relativeTo));
            if (relativeTo == self.parent) return self.localPosition = position;
            return self.position = relativeTo.LocalToGlobal(position);
        }

        public static Vector3 LocalToGlobal(this Transform self, Vector3 v) => self.TransformPoint(v);
        public static Vector3 GlobalToLocal(this Transform self, Vector3 v) => self.InverseTransformPoint(v);
        public static Ray GlobalToLocal(this Transform self, Ray r) => r.GenericTransform(self.GlobalToLocal);
        public static Ray LocalToGlobal(this Transform self, Ray r) => r.GenericTransform(self.LocalToGlobal);


        public static Ray GenericTransform(this Ray r, System.Func<Vector3, Vector3> transformPoints)
        {
            Vector3 a = transformPoints(r.origin), b = transformPoints(r.origin + r.direction);
            return new Ray(a, b - a);
        }

        
        public static double ComputeChainLength(this IEnumerable<Transform> self)
        {
            double ret = 0f;

            Transform last = null;
            foreach (var t in self)
            {
                if (last != null)
                    ret += t.position.Distance(last.position);
                last = t;
            }

            return ret;
        }

    }
}
