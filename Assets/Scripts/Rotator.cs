using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public ConfigurableJoint joint;

    public Vector3[] Rotations;

    private int currentRotationIndex = 0;

    private Quaternion currentRotation => Rotations.Length==0?Quaternion.identity :  Quaternion.Euler(Rotations[currentRotationIndex]);


    private Quaternion originalRotation;
    // Start is called before the first frame update
    void Start()
    {
        originalRotation = joint.gameObject.transform.rotation;
        SetCurrentRotation();
    }



    public void NextRotation()
    {
        ++currentRotationIndex;
        currentRotationIndex %= Rotations.Length;
        SetCurrentRotation();
    }

    private void SetCurrentRotation()
    {
        var curr = currentRotation;

        joint.SetTargetRotation(curr, originalRotation);
    }
}
