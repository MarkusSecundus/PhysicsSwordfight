using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionFix : MonoBehaviour
{
    private Rigidbody rb;



    public struct Snapshot
    {
        public Vector3 CollisionPoint, CollisionNormal, LastRigidbodyPosition;

        public bool IsValidMove(Snapshot frameBefore)
        {
            var traveled = (CollisionPoint - frameBefore.CollisionPoint).normalized;
            return Vector3.Dot(frameBefore.CollisionNormal, traveled) >= 0;
        }
    }
    private Snapshot MakeSnapshot(Collision c) => new Snapshot
    {
        CollisionPoint = c.IterateContacts().Average(c => c.point),
        CollisionNormal = c.IterateContacts().Average(c => c.normal).normalized,
        LastRigidbodyPosition = rb.position,
    };
    private Snapshot LastSnapshot;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        LastSnapshot = MakeSnapshot(collision);
        //Debug.Log($"(fr.{Time.frameCount}) Entering collision: {gameObject.name} -- {collision.gameObject.name} [{collision.contactCount} contacts]");
        Debug.Log($"Collision force: {collision.relativeVelocity.magnitude}, impulse: {collision.impulse.magnitude}");
    }

    private void OnCollisionStay(Collision collision)
    {
        var current = MakeSnapshot(collision);
        //Debug.Log($"Collision.. dot product: {Vector3.Dot(LastSnapshot.CollisionNormal, (LastSnapshot.CollisionPoint - current.CollisionPoint).normalized)}");
        /*foreach(var p in collision.IterateContacts())
        {
            DrawHelpers.DrawWireSphere(p.point, 0.05f, (x,y)=>Debug.DrawLine(x,y,Color.yellow));
            Debug.DrawRay(p.point, p.normal, Color.red);
        }*/
        LastSnapshot = current;
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log($"(fr.{Time.frameCount}) Exiting collision: {gameObject.name} -- {collision.gameObject.name} [{collision.contactCount} contacts]");
        
    }

    private void GetAverageCollisionPoint(Collision col)
    {
    }
}
