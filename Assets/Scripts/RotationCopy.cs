using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCopy : MonoBehaviour
{
    public Transform sourceTransform;

    private ConfigurableJoint targetJoint;

    private Quaternion originalRotation;

    void Start()
    {
        targetJoint = GetComponent<ConfigurableJoint>();

        originalRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        targetJoint.SetTargetRotation(sourceTransform.rotation, originalRotation);
    }
}
