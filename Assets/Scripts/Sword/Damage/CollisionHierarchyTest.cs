using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHierarchyTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Entered {gameObject.name} - colliding {collision.collider.attachedRigidbody.name} - {collision.ThisCollider().attachedRigidbody.name}", this);
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log($"Triggered {gameObject.name}", this);
    }
}
