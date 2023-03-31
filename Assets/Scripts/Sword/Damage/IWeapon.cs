using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWeapon : MonoBehaviour
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never), System.ComponentModel.Browsable(false)]
    public abstract Vector3 CalculateDamage(Collision collision);
    public static IWeapon Get(Collider c) => c.GetComponentInParent<IWeapon>();

    public virtual void OnCollisionEnter(Collision collision)
    {
        var armorPiece = IArmorPiece.Get(collision.collider);
        if (!armorPiece) return;
        

        var damage = this.CalculateDamage(collision);
        damage = armorPiece.CalculateDamage(collision, damage, this);

        armorPiece.BaseDamageable.ChangeHP(-damage.magnitude, this);
    }

}