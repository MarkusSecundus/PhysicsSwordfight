using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUIManager : MonoBehaviour
{
    [SerializeField] ResourceBar HealthBar;
    [SerializeField] BloodStains Bloodstains;

    public float MaxBloodstainsDuration = 5f, BloodstainsBuildup = 0.1f, MinimumHealthLossForBloodstains = 0.2f;

    public void UpdateState(Damageable.HpChangedArgs args)
    {
        float maxHp = args.Target.MaxHP, currentHp = args.Target.HP, dmg = args.DeltaHP;
        float healthLossRatio = dmg/maxHp;

        HealthBar.Value = currentHp / maxHp;

        if(healthLossRatio >= MinimumHealthLossForBloodstains)
        {
            Bloodstains.Show(MaxBloodstainsDuration, buildupTime: BloodstainsBuildup);
            Debug.Log($"Bloodstains: ratio is {healthLossRatio}");
        }
    }
}
