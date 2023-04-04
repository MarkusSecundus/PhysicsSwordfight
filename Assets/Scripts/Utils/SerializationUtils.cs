using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct SerializableRay
{
    [SerializeField]
    public SerializableVector3 origin, direction;

    public SerializableRay(Ray r) => (origin, direction) = (r.origin, r.direction);

    public static implicit operator Ray(SerializableRay r) => new Ray(r.origin, r.direction);
    public static implicit operator SerializableRay(Ray r) => new SerializableRay(r);
}

[System.Serializable]
public struct SerializableVector3
{
    [SerializeField]
    public float x, y, z;

    public SerializableVector3(Vector3 v) => (x,y,z) = (v.x, v.y, v.z);

    public static implicit operator Vector3(SerializableVector3 v) => new Vector3(v.x, v.y, v.z);
    public static implicit operator SerializableVector3(Vector3 v) => new SerializableVector3(v);
}

[System.Serializable]
public struct SerializableVector2
{
    [SerializeField]
    public float x, y;

    public SerializableVector2(Vector2 v) => (x,y) = (v.x, v.y);

    public static implicit operator Vector2(SerializableVector2 v) => new Vector2(v.x, v.y);
    public static implicit operator SerializableVector2(Vector2 v) => new SerializableVector2(v);
}