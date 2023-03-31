using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DullWeapon : IWeapon
{
    public float DamageMultiplier = 1f;
    public override Vector3 CalculateDamage(Collision collision)
    {
        return collision.impulse * DamageMultiplier;
    }
}
