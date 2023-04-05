using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Args = Damageable.HpChangedArgs;

public class DamageReporter : MonoBehaviour
{
    public static DamageReporter Get() => GameObject.FindAnyObjectByType<DamageReporter>();

    public void ReportDamage(Args args)
    {
        Message($"Damaged for {-args.DeltaHP} hp", args.Target);
    }
    public void ReportHeal(Args args)
    {
        Message($"Healed for {args.DeltaHP} hp", args.Target);
    }
    public void ReportDeath(Args args)
    {
        Message("Died!", args.Target);
    }

    private void Message(string s, Object target) => Debug.Log($"{target.name}: {s}", target);
}
