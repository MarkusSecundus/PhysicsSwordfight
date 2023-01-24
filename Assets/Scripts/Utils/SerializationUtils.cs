using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SerializationUtils {}

[System.Serializable]
public struct SerializableRay
{
    public SerializableVector3 origin, direction;

    public SerializableRay(Ray r) => (origin, direction) = (r.origin, r.direction);

    public static implicit operator Ray(SerializableRay r) => new Ray(r.origin, r.direction);
    public static implicit operator SerializableRay(Ray r) => new SerializableRay(r);
}

[System.Serializable]
public struct SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 v) => (x,y,z) = (v.x, v.y, v.z);

    public static implicit operator Vector3(SerializableVector3 v) => new Vector3(v.x, v.y, v.z);
    public static implicit operator SerializableVector3(Vector3 v) => new SerializableVector3(v);
}