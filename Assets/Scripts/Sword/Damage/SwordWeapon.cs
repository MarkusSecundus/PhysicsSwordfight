using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] SwordDescriptor descriptor;
    private void Awake()
    {
        descriptor = descriptor.IfNil(GetComponentInParent<SwordDescriptor>());
    }

    public Vector3 CalculateDamage_impl(Collision collision)
    {


        return default;
    }

}
