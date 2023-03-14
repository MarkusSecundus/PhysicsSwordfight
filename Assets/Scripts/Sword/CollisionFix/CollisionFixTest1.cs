using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionFixTest1 : MonoBehaviour
{
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) => OnCollision(collision);
    private void OnCollisionStay(Collision collision) => OnCollision(collision);
    private void OnCollisionExit(Collision collision) => OnCollision(collision);
    
    private void OnCollision(Collision collision)
    {
        foreach(var c in collision.IterateContacts())
        {
            if(c.normal.Dot(c.impulse) > 0f)
            {
                rb.AddForceAtPosition(-2f*c.impulse, c.point, ForceMode.VelocityChange);
            }
        }
        Debug.Log($"fr.{Time.frameCount} - Fixing {gameObject.name}");
    }
}
