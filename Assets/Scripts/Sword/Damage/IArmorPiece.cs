using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IArmorPiece : MonoBehaviour
{
    [field: SerializeField]
    public virtual Damageable BaseDamageable { get; protected set; }
    public abstract Vector3 CalculateDamage(Collision collision, Vector3 damageAccordingToWeapon, IWeapon weapon);
    public static IArmorPiece Get(Collider c) => !c ? null : c.GetComponentInParent<IArmorPiece>();


    public virtual void OnCollisionEnter(Collision collision) => OnCollision(collision);
    public virtual void OnCollisionStay(Collision collision) => OnCollision(collision);
    public virtual void OnCollisionExit(Collision collision) => OnCollision(collision);


    public virtual void OnCollision(Collision collision, [System.Runtime.CompilerServices.CallerMemberName] string methodName = null)
    {
        Debug.Log($"fr.{Time.frameCount}-{methodName}: impulse{collision.impulse.ToStringPrecise()} relVel{collision.relativeVelocity.ToStringPrecise()}");
    }

    public void ProcessAttack(Collision collision, IWeapon weapon)
    {
        var damage = weapon.CalculateDamage(collision);
        damage = this.CalculateDamage(collision, damage, weapon);
        BaseDamageable.ChangeHP(-damage.magnitude, weapon);
    }
}
