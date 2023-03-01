using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDescriptor : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"Descriptor {gameObject.name}.. target({target}), CoM({centerOfWeight})");
    }

    public Transform SwordCenterOfMass => centerOfWeight.IfNil(target?.SwordCenterOfMass);
    public Transform SwordTip => tipPoint.IfNil(target?.SwordTip);
    public Transform SwordHandleUpHandTarget => upHandTarget.IfNil(target?.SwordHandleUpHandTarget);
    public Transform SwordHandleDownHandTarget => downHandTarget.IfNil(target?.SwordHandleDownHandTarget);
    public Transform SwordBlockPoint => blockPoint.IfNil(target?.blockPoint);

    [SerializeField] private SwordDescriptor target = null;
    [SerializeField] private Transform centerOfWeight=null, tipPoint = null, blockPoint = null, upHandTarget = null, downHandTarget = null;
}
