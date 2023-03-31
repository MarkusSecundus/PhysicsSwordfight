using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArmorPiece : IArmorPiece
{
    public override Damageable BaseDamageable => base.BaseDamageable ??= GetComponentInParent<Damageable>();

    public override Vector3 CalculateDamage(Collision collision, Vector3 damageAccordingToWeapon, IWeapon weapon)
        => damageAccordingToWeapon;
}
