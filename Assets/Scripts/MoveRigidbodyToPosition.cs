using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRigidbodyToPosition : MonoBehaviour
{
    public Transform target;

    public float movementMultiplier = 1f, rotationMultiplier = 1f, rotationDelta = 0.01f;

    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    Quaternion lastRotation = default;
    void FixedUpdate()
    {
        //if (lastRotation == target.rotation) return;
        lastRotation = target.rotation;

        var toMove = (target.position - transform.position);
        var toRotate = (target.rotation.eulerAngles - transform.rotation.eulerAngles)*rotationMultiplier;

        //rb.velocity = toMove;
        //rb.angularVelocity = toRotate;
        //rb.MovePosition(target.position);
        //rb.MovePosition( target.position);
        
        toMove *= toMove.sqrMagnitude;
        toMove *= movementMultiplier;


        //rb.MovePosition(target.position);
        if(true || Vector3.Distance(rb.rotation.eulerAngles, target.rotation.eulerAngles) > rotationDelta)
        {
            rb.angularVelocity = Vector3.zero;
            rb.MoveRotation(target.rotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"ENTERING Collision!");
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log($"Staying in collision! ({this.name} -- {collision.transform.name})");
    }
}
