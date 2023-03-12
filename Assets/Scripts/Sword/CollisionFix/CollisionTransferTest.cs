using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Op;

public class CollisionTransferTest : MonoBehaviour
{
    public Rigidbody Target;

    public ForceMode Mode = ForceMode.Force;

    private Rigidbody rb;

    public float Multiplier = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = Target.position;
    }

    private void OnCollisionEnter(Collision collision) => OnCollision(collision);
    private void OnCollisionStay(Collision collision) => OnCollision(collision);
    private void OnCollisionExit(Collision collision) => OnCollision(collision);

    void OnCollision(Collision collision)
    {
        Debug.Log($"Collision with {collision.gameObject.name} - force: {collision.impulse.ToStringPrecise()}");
        foreach(var c in collision.IterateContacts())
        {
            var point = Target.transform.LocalToGlobal(transform.GlobalToLocal(c.point));

            var force = c.impulse;

            Target.AddForceAtPosition(force * Multiplier, point, Mode);
        }
        //this.rb.position = Target.position;
        this.rb.rotation = Target.rotation;
    }

    private Vector3 lastPosition;
    private void FixedUpdate()
    {
        //rb.position += (Target.position - post_assign(ref lastPosition, Target.position)); rb.rotation = Target.rotation;
    }

}
