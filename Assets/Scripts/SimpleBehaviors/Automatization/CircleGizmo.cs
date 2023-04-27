using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Randomness;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Automatization
{

    public abstract class IPointsSupplier : MonoBehaviour
    {
        public abstract IEnumerable<Vector3> IteratePoints();
        public abstract Vector3 GetRandomPoint(System.Random rand);
        public abstract Vector3 GetRandomPointInVolume(System.Random rand);
    }


    public class CircleGizmo : IPointsSupplier
    {
        public Color Color = Color.black;
        [Range(0, 360f)] public float MinAngle = 0f;
        [Range(0, 360f)] public float MaxAngle = 360f;
        public int Segments = 8;
        public bool ShouldDrawTheGizmo = true;

        private int SegmentIndex(float angle) => Mathf.RoundToInt((angle / 360f) * Segments);

        private void OnDrawGizmos()
        {
            if (!ShouldDrawTheGizmo) return;

            Gizmos.color = Color;

            Vector3 lastPoint = default, beginPoint = default;
            using var it = IteratePoints().GetEnumerator();
            if (it.MoveNext())
                lastPoint = beginPoint = it.Current;

            while (it.MoveNext())
            {
                Gizmos.DrawLine(lastPoint, it.Current);
                lastPoint = it.Current;
            }
            if (!IsCutOff)
                Gizmos.DrawLine(lastPoint, beginPoint);
        }

        private bool IsCutOff => MaxAngle < 360f || MinAngle > 0f;

        public override IEnumerable<Vector3> IteratePoints()
        {
            int minIndex = SegmentIndex(MinAngle), maxIndex = SegmentIndex(MaxAngle);
            var ret = GeometryHelpers.PointsOnCircle(Segments).Select(transform.LocalToGlobal);
            if (IsCutOff) ret = ret.Skip(minIndex).Take(maxIndex - minIndex);
            return ret;
        }

        public override Vector3 GetRandomPoint(System.Random rand)
        {
            float minAngle_radians = MinAngle * Mathf.Deg2Rad, maxAngle_radians = MaxAngle * Mathf.Deg2Rad;
            var randomAngle = rand.NextFloat(minAngle_radians, maxAngle_radians);
            return transform.LocalToGlobal(GeometryHelpers.GetPointOnCircle(randomAngle));
        }

        public override Vector3 GetRandomPointInVolume(System.Random rand)
        {
            float minAngle_radians = MinAngle * Mathf.Deg2Rad, maxAngle_radians = MaxAngle * Mathf.Deg2Rad;
            var randomAngle = rand.NextFloat(minAngle_radians, maxAngle_radians);
            return transform.LocalToGlobal(GeometryHelpers.GetPointOnCircle(randomAngle) * rand.NextFloat(0f, 1f));
        }
    }
}