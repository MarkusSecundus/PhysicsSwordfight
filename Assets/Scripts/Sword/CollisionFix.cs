using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionFix : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    Vector3 lastPosition, collisionBoundary;
    private void FixedUpdate()
    {
        lastPosition = rb.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionBoundary = rb.position;
        Debug.Log($"Entering collision: {gameObject.name} -- {collision.gameObject.name}");
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach(var p in collision.IterateContacts())
        {
            DrawHelpers.DrawWireSphere(p.point, 0.05f, (x,y)=>Debug.DrawLine(x,y,Color.yellow));
            Debug.DrawRay(p.point, p.normal, Color.red);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"Exiting collision: {gameObject.name} -- {collision.gameObject.name}");
    }
}
