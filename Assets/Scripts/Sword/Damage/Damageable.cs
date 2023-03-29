using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : IArmorPiece
{
    [System.Serializable]
    public struct DamagedEventArgs
    {
        public Damageable Target;
        public float DamageDealt;
    }

    [SerializeField] float MaxHP;
    public float HP { get; protected set; }

    public override Damageable BaseDamageable => throw new System.NotImplementedException();

    [SerializeField] UnityEvent OnDeath;
    [SerializeField] UnityEvent<DamagedEventArgs> OnDamaged;

    private void Start()
    {
        HP = MaxHP;
    }

    public void ReportDamage(Vector3 finalDamage, IArmorPiece armorHit, IWeapon weapon)
    {

    }

    public override Vector3 CalculateDamage(Collision collision, Vector3 damageAccordingToWeapon, IWeapon weapon) => damageAccordingToWeapon;
}
