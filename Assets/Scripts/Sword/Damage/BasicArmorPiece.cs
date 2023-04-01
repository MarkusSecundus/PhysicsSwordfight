using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArmorPiece : IArmorPiece
{
    public float MinDamage = 2f;

    public override Damageable BaseDamageable => base.BaseDamageable ??= GetComponentInParent<Damageable>();

    protected override AttackDeclaration? ProcessAttackDeclaration(AttackDeclaration attack)
    {
        if (attack.Damage < MinDamage) return null;
        return attack;
    }
}
