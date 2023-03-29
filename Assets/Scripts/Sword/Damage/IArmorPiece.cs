using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IArmorPiece : MonoBehaviour
{
    public abstract Damageable BaseDamageable { get; }

    public virtual void OnCollisionEnter(Collision collision)
    {
        var weapon = collision.gameObject.GetComponentInParent<IWeapon>();

        var damage = weapon.CalculateDamage(collision);
        damage = this.CalculateDamage(collision, damage, weapon);

        BaseDamageable.ReportDamage(damage, this, weapon);
    }

    public virtual Vector3 CalculateDamage(Collision collision, Vector3 damageAccordingToWeapon, IWeapon weapon) => damageAccordingToWeapon;

    public record Derived(IArmorPiece Base)
    {
        public int Iii { get; init; }
    
        public void f()
        {
            Base.BaseDamageable.ReportDamage(default, null, null);
        }
    }
}
