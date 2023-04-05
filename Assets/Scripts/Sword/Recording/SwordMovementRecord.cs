using Newtonsoft.Json;
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
        [field: SerializeField] public Command Value { get; init; }

    }
    [System.Serializable] public struct Command
    {
        [JsonProperty] public SerializableVector3 LookPoint {get;set;}
        [JsonProperty] public SerializableVector3 AnchorPoint { get; set; }
        [JsonProperty] public SerializableVector3? UpPoint { get; set; }

        public static Command Make(ISwordMovement.MovementCommand c, Transform relativeTo)
        {
            c.LookDirection += c.AnchorPoint;
            if (c.UpDirection != null) c.UpDirection += c.AnchorPoint;

            return new Command
            {
                LookPoint = relativeTo.GlobalToLocal(c.LookDirection),
                AnchorPoint = relativeTo.GlobalToLocal(c.AnchorPoint),
                UpPoint = c.UpDirection == null ? null : relativeTo.GlobalToLocal(c.UpDirection.Value)
            };
        }
        public ISwordMovement.MovementCommand ToCommand(Transform relativeTo)
        {
            LookPoint = relativeTo.LocalToGlobal(LookPoint);
            AnchorPoint = relativeTo.LocalToGlobal(AnchorPoint);
            if(UpPoint != null) UpPoint = relativeTo.LocalToGlobal(UpPoint.Value);

            return new ISwordMovement.MovementCommand
            {
                LookDirection = (Vector3)LookPoint - AnchorPoint,
                AnchorPoint = AnchorPoint,
                UpDirection = UpPoint == null ? null : (Vector3)UpPoint - AnchorPoint
            };
        }
    }
}
