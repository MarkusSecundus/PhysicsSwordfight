using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicImpactWeapon : MonoBehaviour
{
    public float DamageMultiplier = 1f;
    public float SecondsBetweenAttacks = 0.2f;
    protected virtual AttackDeclaration CalculateAttackStats(Collision collision) => new AttackDeclaration 
    {
        Damage = collision.impulse.magnitude * DamageMultiplier,
        AttackerIdentifier = this
    };


    private readonly HashSet<IArmorPiece> targeted = new HashSet<IArmorPiece>();

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!IArmorPiece.TryGet(collision.collider, out var hit)) return;
        if (targeted.TryGetValue(hit, out _)) return;
        targeted.Add(hit);
        ProcessCollision(collision, hit);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!IArmorPiece.TryGet(collision.collider, out var hit)) return;
        this.PerformWithDelay(()=>targeted.Remove(hit), SecondsBetweenAttacks);
    }

    private void ProcessCollision(Collision collision, IArmorPiece hit)
    {
        var attack = CalculateAttackStats(collision);
        hit.ProcessAttack(attack);
    }
}
