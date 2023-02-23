using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCenterOfMass : MonoBehaviour
{
    public Vector3 LocalCenterOfMass = Vector3.zero;
    public bool LogTheAutomaticCenterOfMass;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (LogTheAutomaticCenterOfMass)
            Debug.Log($"{gameObject.name}..Precalculated center of mass was: {rb.centerOfMass}");
        rb.centerOfMass = LocalCenterOfMass;
    }

    private void FixedUpdate()
    {
        if(rb.centerOfMass != LocalCenterOfMass)
        {
            Debug.Log($"{gameObject.name}..Center of mas changed (was {rb.centerOfMass}");
            rb.centerOfMass = LocalCenterOfMass;
        }
    }
}
