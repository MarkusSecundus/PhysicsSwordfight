using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SwordMovementRecord 
{
    public Frame[] Frames { get; init; }
    [System.Serializable] public struct Frame
    {
        [field: SerializeField] public double Timestamp { get; init; }
        [field: SerializeField] public ISwordMovement.MovementCommand Value { get; init; }
    }
}
