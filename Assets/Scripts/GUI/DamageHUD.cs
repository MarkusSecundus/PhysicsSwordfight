using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Args = Damageable.HpChangedArgs;

public class DamageHUD : MonoBehaviour
{
    public static DamageHUD Get() => GameObject.FindAnyObjectByType<DamageHUD>();

    [SerializeField] ResourceBar HealthBar;
    [SerializeField] BloodStains BloodstainsInstancer;

    [SerializeField] private Damageable _target;
    public Damageable Target { get=>_target; set {
            _target = value;
            lastHp = value.HP;
        } }


    [SerializeField] BloodstainsConfig Bloodstains = BloodstainsConfig.Default;

    [System.Serializable]
    public struct BloodstainsConfig
    {
        public float MaxBloodstainsDuration, BloodstainsBuildup, MinimumHealthLossForBloodstains;
        public static BloodstainsConfig Default => new(){ MaxBloodstainsDuration = 5f, BloodstainsBuildup = 0.1f, MinimumHealthLossForBloodstains = 0.2f };
    }

    float lastHp;
    private void Start()
    {
        if(Target.IsNotNil()) lastHp = Target.HP;
    }
    private void Update()
    {
        if (Target.IsNil()) return;
        var deltaHp = Target.HP - lastHp;
        UpdateHealthbar(Target);
        TryShowBloodstains(deltaHp, Target.MaxHP);
        lastHp = Target.HP;
    }

    void UpdateHealthbar(Damageable target)
    {
        HealthBar.Value = target.HP / target.MaxHP;
    }

    void TryShowBloodstains(float deltaHp, float maxHp)
    {
        float healthLossRatio = -deltaHp / maxHp;
        if (healthLossRatio >= Bloodstains.MinimumHealthLossForBloodstains)
        {
            BloodstainsInstancer.Show(Bloodstains.MaxBloodstainsDuration, buildupTime: Bloodstains.BloodstainsBuildup);
        }
    }

}
