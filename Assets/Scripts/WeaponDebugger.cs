using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDebugger : MonoBehaviour
{
    [SerializeField]
    Transform actualDebugger;

    public void AdjustPosition(ConfigurableJoint weaponToDebug)
    {
        var anchorShift = weaponToDebug.anchor;//weaponToDebug.transform.parent.TransformPoint(weaponToDebug.transform.InverseTransformPoint(weaponToDebug.anchor));
        Debug.Log($"orig: {weaponToDebug.anchor} | trans: {anchorShift}");
        transform.position = weaponToDebug.transform.position - anchorShift;
        actualDebugger.localPosition = Vector3.zero;// anchorShift;
    }

    public void RotateDebugger(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}
