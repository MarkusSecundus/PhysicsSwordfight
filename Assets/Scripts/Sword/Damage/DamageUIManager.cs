using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Args = Damageable.HpChangedArgs;

public class DamageUIManager : MonoBehaviour
{
    [SerializeField] ResourceBar HealthBar;
    [SerializeField] BloodStains BloodstainsInstancer;


    [SerializeField] BloodstainsConfig Bloodstains = BloodstainsConfig.Default;

    [System.Serializable]
    public struct BloodstainsConfig
    {
        public float MaxBloodstainsDuration, BloodstainsBuildup, MinimumHealthLossForBloodstains;
        public static BloodstainsConfig Default => new(){ MaxBloodstainsDuration = 5f, BloodstainsBuildup = 0.1f, MinimumHealthLossForBloodstains = 0.2f };
    }


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

    public void UpdateState(Args args)
    {
        HealthBar.Value = args.Target.HP / args.Target.MaxHP;
    }

    public void TryShowBloodstains(Args args)
    {
        float healthLossRatio = -args.DeltaHP / args.Target.MaxHP;
        if (healthLossRatio >= Bloodstains.MinimumHealthLossForBloodstains)
        {
            BloodstainsInstancer.Show(Bloodstains.MaxBloodstainsDuration, buildupTime: Bloodstains.BloodstainsBuildup);
        }
    }

    private void Message(string s, Object target) => Debug.Log($"{target.name}: {s}", target);
}
