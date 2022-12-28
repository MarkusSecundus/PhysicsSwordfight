using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePositionHelper : MonoBehaviour
{
    public Vector3 relative, absolute, relTransformed, relInverseTransformed, absTransformed, absInverseTransformed;

    public TransformationPolicy policy;

    // Update is called once per frame
    void Update()
    {
        relative = transform.localPosition;
        absolute = transform.position;
        relTransformed = transform.parent.Transform(relative, policy);
        relInverseTransformed = transform.parent.InverseTransform(relative, policy);
        absTransformed = transform.parent.Transform(absolute, policy);
        absInverseTransformed = transform.parent.InverseTransform(absolute, policy);
    }
}
