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

    [SerializeField] UnityEvent OnDeath;
    [SerializeField] OnHpChangedEvent OnDamaged;
    [SerializeField] OnHpChangedEvent OnHealed;

    private void Start()
    {
        HP = MaxHP;
    }

    public void ChangeHP(float deltaHP, [MaybeNull] IWeapon cause)
    {
        var resultHP = Mathf.Clamp(HP + deltaHP, MinHP, MaxHP);
        //deltaHP = resultHP - HP;

        var message = $"{this.name} ";
        OnHpChangedEvent eventToInvoke;
        HP = resultHP;
        if(deltaHP <= 0)
        {
            message += $"damaged for {-deltaHP} hp";
            eventToInvoke = OnDamaged;
        }
        else
        {
            message += $"healed for {deltaHP} hp";
            eventToInvoke = OnHealed;
        }
        if (cause != null) message += $" by {cause.name}";

        Message(message);

        eventToInvoke?.Invoke(new HpChangedArgs { Target = this, DeltaHP = deltaHP});
        if (resultHP <= 0)
        {
            Message($"{this.name} died!");
            OnDeath?.Invoke();
        }
    }

    void Message(string message) => Debug.Log(message, this);
}
