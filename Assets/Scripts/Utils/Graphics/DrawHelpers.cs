using MarkusSecundus.PhysicsSwordfight.Utils.Geometry;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Graphics
{
    /// <summary>
    /// Static class providing methods for drawing shapes
    /// </summary>
    public static class DrawHelpers
    {
        /// <summary>
        /// Delegate for drawing lines
        /// </summary>
        /// <typeparam name="TVect">Vector type</typeparam>
        /// <param name="lineBegin">origin point of the line</param>
        /// <param name="lineEnd">end point of the line</param>
        public delegate void LineDrawer<TVect>(TVect lineBegin, TVect lineEnd);

        /// <summary>
        /// Draw 2D wire circle with center in origin.
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="segments">Number of segments of the circle</param>
        /// <param name="drawLine">Function to draw the lines</param>
        public static void DrawWireCircle(float radius, int segments, LineDrawer<Vector2> drawLine)
        {
            Vector2 v = new Vector2(radius, 0);
            foreach (Vector2 w in SphereGeometryHelpers.PointsOnCircle(segments, v))
            {
                drawLine(v, w);
                v = w;
            }
        }

        /// <summary>
        /// Draw 2D wire sphere at given position in space
        /// </summary>
        /// <param name="center">Center of the sphere</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="drawLine">Function for drawing the lines</param>
        /// <param name="segments">Number of segments of each circle component</param>
        /// <param name="circles">How many circles the sphere consists of</param>
        public static void DrawWireSphere(Vector3 center, float radius, LineDrawer<Vector3> drawLine, int? segments = null, int? circles = null)
        {
            int segm = segments ?? computeCircleSegments(radius);
            int circs = circles ?? computeSphereCircles(radius);

            var rot = Matrix4x4.Rotate(Quaternion.Euler(0, 0, NumericConstants.MaxDegree / 2 / circs));
            DrawWireCircle(radius, segm, (v, w) =>
            {
                Vector3 a = v.x0y(), b = w.x0y();
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

        /// <summary>
        /// Visualize a plane segment arround given point in space. Draws the lines from points on sides - uses <c>2*diameter</c> lines.
        /// </summary>
        /// <param name="plane">Plane to visualize</param>
        /// <param name="center">Point on a plane to serve as center of the visualization</param>
        /// <param name="drawLine">Function for drawing the lines</param>
        /// <param name="diameter">Size of the visualized plane segment</param>
        /// <param name="segments">How detailed the visualization is</param>
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

        /// <summary>
        /// Visualize a plane segment arround given point in space. Draws lines inside the plane - uses <c>diameter^2</c> lines.
        /// </summary>
        /// <param name="plane">Plane to visualize</param>
        /// <param name="center">Point on a plane to serve as center of the visualization</param>
        /// <param name="drawLine">Function for drawing the lines</param>
        /// <param name="diameter">Size of the visualized plane segment</param>
        /// <param name="segments">How detailed the visualization is</param>
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
