using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDrawTest : MonoBehaviour
{
    public MeshFilter filter;

    public Vector2 begin, steps = new Vector2(0.2f, 0.2f);
    public Vector2Int stepsCount = new Vector2Int(20, 20);

    public float divider = 1f, gizmosRadius = 0.01f;

    public bool gizmos = false;

    public Vector3[] vertices;
    public int[] triangles;
 
    // Start is called before the first frame update
    void Start()
    {
        MeshUtils.CreateFlatMeshData(begin, steps, stepsCount, out vertices, out triangles);
        filter.mesh = MeshUtils.MeshFromData(vertices, triangles);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            vertices = vertices.AdjustHeights((x, y) => (x * x + y * y)/divider);
            filter.mesh.vertices = vertices;
            filter.mesh.RecalculateNormals();
        }
    }

    private void OnDrawGizmos()
    {
        if (!gizmos) return;
        if (vertices == null) return;

        foreach(var v in vertices)
            Gizmos.DrawSphere(transform.TransformPoint(v), gizmosRadius);
    }




    private void SetUpMesh()
    {
        var m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();

        filter.mesh = m;
    }
}
