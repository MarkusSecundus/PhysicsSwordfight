using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SwordWeapon : IWeapon
{
    [SerializeField] SwordDescriptor descriptor;

    [SerializeField] float EdgeSharpness = 1f, SideDullingExponent = 0.3f;

    private void Awake()
    {
        descriptor = GetComponentInParent<SwordDescriptor>();
    }

    public override Vector3 CalculateDamage(Collision collision)
    {
        if(descriptor.EdgeDirections.Count <= 0)
        {
            Debug.Log($"No sword edges defined!", descriptor);
            return default;
        }
        return descriptor.EdgeDirections.Select(computeDamage).Maximum(v=>v.magnitude);

        Vector3 computeDamage(Vector3 edgeDirection)
        {
            var directionRatio = Mathf.Abs(edgeDirection.normalized.Dot(collision.impulse.normalized));
            directionRatio = Mathf.Pow(directionRatio, SideDullingExponent);
            var sharpnessRatio = directionRatio * EdgeSharpness;
            return collision.impulse * sharpnessRatio;
        }
    }

}
