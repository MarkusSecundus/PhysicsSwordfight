using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackDeclaration
{
    [System.Serializable] public struct ImpactPointData
    {
        [field: SerializeField] public Vector3 Point { get; init; }
        [field: SerializeField] public Vector3 Normal { get; init; }
    }

    [field: SerializeField] public float Damage { get; init; }
    [field: SerializeField] public Object AttackerIdentifier { get; init; }
    public string AttackerName => AttackerIdentifier.name;
    [field: SerializeField] public ImpactPointData ImpactPoint { get; init; }
}

public static class AttackDeclarationExtensions
{
    public static AttackDeclaration.ImpactPointData AsImpactPointData(this ContactPoint self) => new AttackDeclaration.ImpactPointData { Point = self.point, Normal = self.normal };
}