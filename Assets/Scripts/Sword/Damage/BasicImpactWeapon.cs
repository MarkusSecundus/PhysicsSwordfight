using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ImpactWeapon<TStats> : MonoBehaviour
{
    public float SecondsBetweenAttacks = 0.2f;

    public TStats Stats;

    public ExceptionsList Exceptions;

    [System.Serializable] public class ExceptionsList : SerializableDictionary<Damageable, TStats> { }
    protected abstract AttackDeclaration CalculateAttackStats(Collision collision, TStats stats);


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
        var statsToUse = Exceptions.Values.GetValueOrDefault(hit.BaseDamageable, Stats);
        var attack = CalculateAttackStats(collision, statsToUse);
        hit.ProcessAttack(attack);
    }
}

public class BasicImpactWeapon : ImpactWeapon<BasicImpactWeapon.StatsDefinition>
{
    [System.Serializable] public struct StatsDefinition
    {
        public float DamageMultiplier;
    }
    protected override AttackDeclaration CalculateAttackStats(Collision collision, StatsDefinition stats) => new AttackDeclaration 
    {
        Damage = collision.impulse.magnitude * stats.DamageMultiplier,
        AttackerIdentifier = this,
        ImpactPoint = collision.GetContact(0).AsImpactPointData()
    };
}
