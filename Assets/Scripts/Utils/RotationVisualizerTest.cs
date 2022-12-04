using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationVisualizerTest : MonoBehaviour
{
    public Transform point;
    public Transform toRotate;

    public Quaternion angle;
    public Vector3 lookAt, up;

    // Update is called once per frame
    void Update()
    {
        angle = Quaternion.LookRotation(lookAt, up);
        Debug.DrawLine(toRotate.position, toRotate.position + lookAt, Color.blue);
        Debug.DrawLine(toRotate.position, toRotate.position + up, Color.cyan);
        void Draw()
        {
            DrawHelpers.DrawWireSphere(point.position, 0.05f, (a, b) => Debug.DrawLine(a, b, Color.red), 10, 16);
            Debug.DrawLine(toRotate.position, point.position, Color.yellow);
        }
        var originalRotation = toRotate.localRotation;
        var originalPos = point.position - toRotate.position;
        Draw();
        toRotate.localRotation = originalRotation * angle;
        Draw();
        Debug.Log($"dot: {(point.position - toRotate.position).Dot(originalPos)}");
        toRotate.localRotation = originalRotation;
    }
}
