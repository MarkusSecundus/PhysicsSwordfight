using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public Vector3 CalculateDamage_impl(Collision collision);

    public static Vector3 CalculateGenericDamage(Collision collision) => collision.impulse;
}

public static class WeaponExtensions
{
    public static Vector3 CalculateDamage(this IWeapon self, Collision collision) => self?.CalculateDamage_impl(collision) ?? IWeapon.CalculateGenericDamage(collision);
}