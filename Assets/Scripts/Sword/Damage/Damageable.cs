using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;


public class Damageable : MonoBehaviour
{
    [System.Serializable] public class OnHpChangedEvent : UnityEvent<HpChangedArgs> { }

    [System.Serializable]
    public struct HpChangedArgs
    {
        public Damageable Target;
        public float DeltaHP;
    }

    public const float MinHP = 0;
    [field: SerializeField] public float MaxHP { get; set; } = 1f;
    public float HP { get; private set; }

    public bool DestroySelfOnDeath = false;

    [SerializeField] OnHpChangedEvent OnUpdated;
    [SerializeField] OnHpChangedEvent OnDamaged;
    [SerializeField] OnHpChangedEvent OnHealed;
    [SerializeField] OnHpChangedEvent OnDeath;

    private void Start()
    {
        HP = MaxHP;
        OnUpdated.Invoke(new HpChangedArgs { Target = this, DeltaHP = 0});
    }

    public void ChangeHP(float deltaHP)
    {
        HP = Mathf.Clamp(HP + deltaHP, MinHP, MaxHP);

        var eventArgs = new HpChangedArgs { Target = this, DeltaHP = deltaHP };

        if (deltaHP < 0) OnDamaged.Invoke(eventArgs);
        else if (deltaHP > 0) OnHealed.Invoke(eventArgs);
        OnUpdated.Invoke(eventArgs);
        if (HP <= 0)
        {
            OnDeath.Invoke(eventArgs);
            if (DestroySelfOnDeath)
                Destroy(gameObject);
        }
    }

    void Message(string message) => Debug.Log(message, this);
}
