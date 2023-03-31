using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IArmorPiece : MonoBehaviour
{
    [field: SerializeField]
    public virtual Damageable BaseDamageable { get; protected set; }

    public void OnCollisionEnter(Collision collision)
    {
        var weapon = IWeapon.Get(collision.collider);
        if (!weapon) return;


        var damage = weapon.CalculateDamage(collision);
        damage = this.CalculateDamage(collision, damage, weapon);

        BaseDamageable.ChangeHP(-damage.magnitude, weapon);
    }

    public abstract Vector3 CalculateDamage(Collision collision, Vector3 damageAccordingToWeapon, IWeapon weapon);

    public static IArmorPiece Get(Collider c) => !c?null: c.GetComponentInParent<IArmorPiece>();
}
