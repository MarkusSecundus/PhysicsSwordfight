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

    public int WindowSize = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = Target.position;
    }

    private void OnCollisionEnter(Collision collision) => OnCollision(collision);
    private void OnCollisionStay(Collision collision) => OnCollision(collision);
    private void OnCollisionExit(Collision collision) => OnCollision(collision);

    void OnCollision(Collision collision)
    {
        //Debug.Log($"Collision with {collision.gameObject.name} - force: {collision.impulse.ToStringPrecise()}");

        WorksQuiteOk(collision);
        //WithoutForceTranslation(collision);
        //Debug.Log($"velocity: {rb.velocity} - {rb.angularVelocity}");
    }

    long shouldApplyVelocityDiffCounter = 0;
    void Test()
    {
        shouldApplyVelocityDiffCounter = WindowSize;
    }
    void AlsoNotMuchGood(Collision collision)
    {
        var sum = Vector3.zero;
        foreach (var c in collision.IterateContacts()) sum += c.point;
        var point = Target.transform.LocalToGlobal(transform.GlobalToLocal(sum));
        Target.AddForceAtPosition(collision.impulse * Multiplier, point, Mode);
    }
    void DoesntWorkAtAll(Collision collision)
    {
        Target.AddForce(collision.impulse, Mode);
    }

    void WithoutForceTranslation(Collision collision)
    {
        foreach (var c in collision.IterateContacts())
        {
            var point = c.point;

            var force = c.impulse;

            Target.AddForceAtPosition(force * Multiplier, point, Mode);
        }
    }
    void WorksQuiteOk(Collision collision)
    {
        foreach (var c in collision.IterateContacts())
        {
            var point = Target.transform.LocalToGlobal(transform.GlobalToLocal(c.point));

            var force = c.impulse;

            Target.AddForceAtPosition(force * Multiplier, point, Mode);
        }
    }


    Vector3 velocity, angularVelocity;
    private Vector3 lastPosition;
    private void FixedUpdate()
    {
        if(shouldApplyVelocityDiffCounter-- > 0)
        {
            //Target.velocity += rb.velocity - post_assign(ref velocity, rb.velocity);
            //Target.angularVelocity += rb.angularVelocity - post_assign(ref angularVelocity, rb.angularVelocity);
        }


        //rb.position += (Target.position - post_assign(ref lastPosition, Target.position)); rb.rotation = Target.rotation;
    }

}
