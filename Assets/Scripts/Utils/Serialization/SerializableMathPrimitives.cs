using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Serialization
{
    /// <summary>
    /// Variant of <see cref="Ray"/> that doesn't contain cyclic references and thus can be serialized by Newtonsoft.Json machinery
    /// </summary>
    [System.Serializable]
    public struct SerializableRay
    {
        /// <summary>
        /// Begin point of the ray
        /// </summary>
        [SerializeField] public SerializableVector3 origin;
        /// <summary>
        /// Direction of the ray
        /// </summary>
        [SerializeField] public SerializableVector3 direction;

        public SerializableRay(ScaledRay r) => (origin, direction) = (r.origin, r.direction);

        public static implicit operator ScaledRay(SerializableRay r) => ScaledRay.FromDirection(r.origin, r.direction);
        public static implicit operator SerializableRay(ScaledRay r) => new SerializableRay(r);
    }

    /// <summary>
    /// Variant of <see cref="Vector3"/> that doesn't contain cyclic references and thus can be serialized by Newtonsoft.Json machinery
    /// </summary>
    [System.Serializable]
    public struct SerializableVector3
    {
        [SerializeField]public float x;
        [SerializeField]public float y;
        [SerializeField]public float z;

        public SerializableVector3(Vector3 v) => (x, y, z) = (v.x, v.y, v.z);

        public static implicit operator Vector3(SerializableVector3 v) => new Vector3(v.x, v.y, v.z);
        public static implicit operator SerializableVector3(Vector3 v) => new SerializableVector3(v);
    }

    /// <summary>
    /// Variant of <see cref="Vector2"/> that doesn't contain cyclic references and thus can be serialized by Newtonsoft.Json machinery
    /// </summary>
    [System.Serializable]
    public struct SerializableVector2
    {
        [SerializeField]public float x;
        [SerializeField]public float y;

        public SerializableVector2(Vector2 v) => (x, y) = (v.x, v.y);

        public static implicit operator Vector2(SerializableVector2 v) => new Vector2(v.x, v.y);
        public static implicit operator SerializableVector2(Vector2 v) => new SerializableVector2(v);
    }
}