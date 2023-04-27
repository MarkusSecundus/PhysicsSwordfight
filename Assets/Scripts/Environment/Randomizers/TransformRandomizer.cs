using MarkusSecundus.PhysicsSwordfight.Automatization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Environment.Randomization
{

    public class TransformRandomizer : MonoBehaviour, IRandomizer
    {
        public Vector2 ScaleMin = Vector2.one, ScaleMax = Vector2.one;
        public Vector3 RotationMin = Vector3.zero, RotationMax = new Vector3(0, 360f, 0);
        public Vector3 PlaceOffsetMin = Vector3.zero, PlaceOffsetMax = Vector3.zero;
        public void Randomize(System.Random random)
        {
            var scale = random.NextVector2(ScaleMin, ScaleMax).xyx();
            var rotation = random.NextVector3(RotationMin, RotationMax);

            transform.localScale = transform.localScale.MultiplyElems(scale);
            transform.localRotation *= Quaternion.Euler(rotation);
            transform.position += random.NextVector3(PlaceOffsetMin, PlaceOffsetMax);
        }
    }
}