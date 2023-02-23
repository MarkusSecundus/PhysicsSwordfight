using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointRotationHelp : MonoBehaviour
{
    public ConfigurableJoint joint;

    public Vector3 targetRotation = Vector3.zero;

    public Vector3 debug, debugLocal;

    private Quaternion originalRotation, originalLocalRotation;

    // Start is called before the first frame update
    void Start()
    {
        joint ??= GetComponent<ConfigurableJoint>();
        originalRotation = joint.transform.rotation;
        originalLocalRotation = joint.transform.localRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ConfigurableJointExtensions.SetTargetRotationInternal(joint, Quaternion.Euler(targetRotation), joint.configuredInWorldSpace ? originalRotation : originalLocalRotation, joint.configuredInWorldSpace ? Space.World : Space.Self);
        debug = joint.transform.eulerAngles;
        debugLocal = joint.transform.localEulerAngles;
    }
}
