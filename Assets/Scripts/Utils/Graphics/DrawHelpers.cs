using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Graphics
{
    public static class DrawHelpers
    {
        public delegate void LineDrawer<TVect>(TVect lineBegin, TVect lineEnd);

        public static void DrawDirectedLine(this LineDrawer<Vector3> self, Vector3 begin, Vector3 direction)
            => self(begin, begin + direction);

        public static void DrawWireCircle(float radius, int segments, LineDrawer<Vector2> drawLine)
        {
            Vector2 v = new Vector2(radius, 0);
            foreach (Vector2 w in GeometryHelpers.PointsOnCircle(segments, v))
            {
                drawLine(v, w);
                v = w;
            }
        }

        public static void DrawWireSphere(Vector3 center, float radius, LineDrawer<Vector3> drawLine, int? segments = null, int? circles = null)
        {
            int segm = segments ?? computeCircleSegments(radius);
            int circs = circles ?? computeSphereCircles(radius);

            var rot = Matrix4x4.Rotate(Quaternion.Euler(0, 0, NumericConstants.MaxDegree / 2 / circs));
            DrawWireCircle(radius, segm, (v, w) =>
            {
                Vector3 a = v.x0z(), b = w.x0z();
                for (int t = 0; t < circs; ++t)
                {
                    drawLine(a + center, b + center);
                    a = rot * a;
                    b = rot * b;
                }
            });

            int computeCircleSegments(float radius) => (int)Mathf.Max(6, Mathf.Ceil(Mathf.Sqrt(radius) * 36f));
            int computeSphereCircles(float radius) => (int)Mathf.Max(2, Mathf.Ceil(Mathf.Sqrt(radius) * 18f));
        }


        public static void DrawPlaneSegment(Plane plane, Vector3 center, LineDrawer<Vector3> drawLine, Vector2 diameter = default, int segments = 24)
        {
            if (diameter == default) diameter = new Vector2(1, 1);

            var step = diameter / (segments);

            var bs = plane.GetBase();

            Vector2 begin = -diameter / 2;

            for (int x = 1; x < segments; ++x)
            {
                var b = begin + step.x0() * x;
                var e = begin + step.x0() * x + step._0y() * segments;
                draw(b, e);
            }

            for (int y = 1; y < segments; ++y)
            {
                var b = begin + step._0y() * y;
                var e = begin + step._0y() * y + step.x0() * segments;
                draw(b, e);
            }

            void draw(Vector2 a, Vector2 b)
            {
                drawLine(bs.GetBasedVector(a) + center, bs.GetBasedVector(b) + center);
            }
        }


        public static void DrawPlaneSegmentInterstepped(Plane plane, Vector3 center, LineDrawer<Vector3> drawLine, Vector2 diameter = default, int segments = 24)
        {
            if (diameter == default) diameter = new Vector2(1, 1);

            var step = diameter / (segments);

            var bs = plane.GetBase();

            Vector2 begin = -diameter / 2;

            for (int x = 0; x < segments - 1; ++x)
            {
                for (int y = 0; y < segments - 1; ++y)
                {
                    var b = begin + step.MultiplyElems(x, y);
                    var bx = b + step.x0();
                    var by = b + step._0y();
                    draw(b, bx);
                    draw(b, by);
                }
            }

            void draw(Vector2 a, Vector2 b)
            {
                drawLine(bs.GetBasedVector(a) + center, bs.GetBasedVector(b) + center);
            }
        }
    }
}
