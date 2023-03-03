using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDescriptor : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"Descriptor {gameObject.name}.. target({target}), CoM({centerOfWeight})");
    }

    public Transform SwordAnchor => anchor = anchor.IfNil(target?.SwordAnchor);
    public Transform SwordCenterOfMass => centerOfWeight = centerOfWeight.IfNil(target?.SwordCenterOfMass);
    public Transform SwordTip => tipPoint = tipPoint.IfNil(target?.SwordTip);
    public Transform SwordHandleUpHandTarget => upHandTarget = upHandTarget.IfNil(target?.SwordHandleUpHandTarget);
    public Transform SwordHandleDownHandTarget => downHandTarget = downHandTarget.IfNil(target?.SwordHandleDownHandTarget);
    public Transform SwordBlockPoint => blockPoint = blockPoint.IfNil(target?.blockPoint);

    [SerializeField] private SwordDescriptor target = null;
    [SerializeField] private Transform anchor = null, centerOfWeight=null, tipPoint = null, blockPoint = null, upHandTarget = null, downHandTarget = null;
}
