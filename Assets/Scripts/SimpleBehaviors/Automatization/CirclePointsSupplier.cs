using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Geometry;
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
    /// <summary>
    /// Provides points of a circle or circle segment. Modify the object's <see cref="Transform"/> to transform the circle.
    /// </summary>
    public class CirclePointsSupplier : IPointsSupplier
    {
        /// <summary>
        /// Color of the gizmo visualization. To be used from editor.
        /// </summary>
        public Color Color = Color.black;
        /// <summary>
        /// Whether the shape should be visualized in editor.
        /// </summary>
        public bool ShouldDrawTheGizmo = true;
        /// <summary>
        /// Angle where the circle segment starts
        /// </summary>
        [Range(0, 360f)] public float MinAngle = 0f;
        /// <summary>
        /// Angle where the circle segment ends
        /// </summary>
        [Range(0, 360f)] public float MaxAngle = 360f;
        /// <summary>
        /// How many segments to make the whole circle.
        /// </summary>
        public int Segments = 8;

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

        /// <inheritdoc/>
        public override IEnumerable<Vector3> IteratePoints()
        {
            int minIndex = SegmentIndex(MinAngle), maxIndex = SegmentIndex(MaxAngle);
            var ret = SphereGeometryHelpers.PointsOnCircle(Segments).Select(transform.LocalToGlobal);
            if (IsCutOff) ret = ret.Skip(minIndex).Take(maxIndex - minIndex);
            return ret;
        }

        /// <inheritdoc/>
        public override Vector3 GetRandomPoint(System.Random rand)
        {
            float minAngle_radians = MinAngle * Mathf.Deg2Rad, maxAngle_radians = MaxAngle * Mathf.Deg2Rad;
            var randomAngle = rand.NextFloat(minAngle_radians, maxAngle_radians);
            return transform.LocalToGlobal(SphereGeometryHelpers.GetPointOnCircle(randomAngle));
        }

        /// <inheritdoc/>
        public override Vector3 GetRandomPointInVolume(System.Random rand)
        {
            float minAngle_radians = MinAngle * Mathf.Deg2Rad, maxAngle_radians = MaxAngle * Mathf.Deg2Rad;
            var randomAngle = rand.NextFloat(minAngle_radians, maxAngle_radians);
            return transform.LocalToGlobal(SphereGeometryHelpers.GetPointOnCircle(randomAngle) * rand.NextFloat(0f, 1f));
        }
    }
}