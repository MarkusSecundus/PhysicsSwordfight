using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaboloidSet : MonoBehaviour
{
    public RenderConfig renderConfig;

    [System.Serializable]
    public class RenderConfig
    {
        public MeshFilter filter;

        public Vector2 begin, steps = new Vector2(0.2f, 0.2f);
        public Vector2Int stepsCount = new Vector2Int(20, 20);

        public bool shouldRender => filter != null;

        private Vector3[] vertices;
        private int[] triangles;

        public void SetUpRender(ParaboloidSet self)
        {
            if (!shouldRender) return;

            MeshUtils.CreateFlatMeshData(begin, steps, stepsCount, out vertices, out triangles);
            filter.mesh = MeshUtils.MeshFromData(vertices, triangles);

            SetHeights(getFunc(self));
            filter.mesh.vertices = vertices;filter.mesh.RecalculateNormals();
        }

        private void SetHeights(Func<float, float, float> f)
        {
            for(int i = 0; i < vertices.Length; ++i)
            {
                var v = vertices[i];
                vertices[i] = new Vector3(v.x, v.y, f(v.x, v.y));
            }
        }

        private Func<float, float, float> getFunc(ParaboloidSet self) 
            => (x, y) => (x * x + y * y) / self.divider;
    }


    public float divider;
    public float beginInterpolation, endInterpolation;



    public void Start()
    {
        renderConfig?.SetUpRender(this);
    }

    public (Vector3? First, Vector3? Second) Intersect(Ray r)
    {
        r = transform.TransformRay(r);
        //r.origin -= transform.position;
        DebugDrawUtils.DrawRay(r, Color.yellow);

        return GeometryUtils.IntersectParaboloid(r, transform.position, divider);
    }

}
