using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    public struct TransformSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;

        public TransformSnapshot(Transform t)
        {
            (position, rotation, localScale) = (t.position, t.rotation, t.localScale);
        }
        public TransformSnapshot(Rigidbody r)
        {
            (position, rotation, localScale) = (r.position, r.rotation, default);
        }

        public void SetTo(Transform t)
        {
            (t.position, t.rotation) = (position, rotation);
            if (localScale != default) t.localScale = localScale;
        }
        public void SetTo(Rigidbody r)
        {
            (r.position, r.rotation) = (position, rotation);
        }
    }

}
