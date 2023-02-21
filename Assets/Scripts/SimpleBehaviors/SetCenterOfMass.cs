using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCenterOfMass : MonoBehaviour
{
    public Vector3 LocalCenterOfMass = Vector3.zero;
    public bool LogTheAutomaticCenterOfMass;
    void Start()
    {
        var rb = GetComponent<Rigidbody>();
        if (LogTheAutomaticCenterOfMass)
            Debug.Log($"{gameObject.name}..Precalculated center of mass was: {rb.centerOfMass}");
        rb.centerOfMass = LocalCenterOfMass;
    }
}
