using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRandomizer : MonoBehaviour, IRandomizer
{
    public Vector2 ScaleMin = Vector2.one, ScaleMax = Vector2.one;
    public Vector3 RotationMin = Vector3.one, RotationMax = Vector3.one;
    public Vector3 PlaceOffsetMin = Vector3.one, PlaceOffsetMax = Vector3.one;
    public void Randomize(System.Random random)
    {
        var scale = random.NextVector2(ScaleMin, ScaleMax).xyx();
        var rotation = random.NextVector3(RotationMin, RotationMax);

        transform.localScale = transform.localScale.MultiplyElems(scale);
        transform.localRotation *= Quaternion.Euler(rotation);
        transform.position += random.NextVector3(PlaceOffsetMin, PlaceOffsetMax);
    }
}
