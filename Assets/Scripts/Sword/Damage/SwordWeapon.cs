using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SwordWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] SwordDescriptor descriptor;

    [SerializeField] float EdgeSharpness = 1f, SideDullingExponent = 0.3f;

    private void Awake()
    {
        descriptor = descriptor.IfNil(GetComponentInParent<SwordDescriptor>());
    }

    public Vector3 CalculateDamage_impl(Collision collision)
    {
        return descriptor.EdgeDirections.Select(computeDamage).Maximum(v=>v.magnitude);

        Vector3 computeDamage(Vector3 edgeDirection)
        {
            var directionRatio = edgeDirection.normalized.Dot(collision.impulse.normalized);
            directionRatio = Mathf.Abs(directionRatio);
            directionRatio = Mathf.Pow(directionRatio, SideDullingExponent);
            var sharpnessRatio = directionRatio * EdgeSharpness;
            return collision.impulse * sharpnessRatio;
        }
    }

}
