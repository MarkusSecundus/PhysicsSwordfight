using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackDeclaration
{
    [field: SerializeField] public float Damage { get; init; }
    [field: SerializeField] public Object AttackerIdentifier { get; init; }
    public string AttackerName => AttackerIdentifier.name;
}
