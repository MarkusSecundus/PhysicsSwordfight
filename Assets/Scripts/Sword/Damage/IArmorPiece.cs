using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class IArmorPiece : MonoBehaviour
{
    [field: SerializeField]
    public virtual Damageable BaseDamageable { get; protected set; }
    protected abstract AttackDeclaration? ProcessAttackDeclaration(AttackDeclaration attack);


    public static bool TryGet(Collider c, out IArmorPiece ret)
    {
        ret = null;
        if(c && (ret = c.GetComponentInParent<IArmorPiece>())!= null)
            return true;
        return false;
    }

    public void ProcessAttack(AttackDeclaration attack)
    {
        var processed = ProcessAttackDeclaration(attack);
        if(processed != null)
        {
            attack = processed.Value;
            BaseDamageable.ChangeHP(-attack.Damage);
        }
    }
}
