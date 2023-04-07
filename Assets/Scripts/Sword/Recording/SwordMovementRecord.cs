using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SwordMovementRecord 
{
    [System.Serializable] public class Segment
    {
        [JsonProperty] public Frame[] Frames { get; init; }
        [JsonProperty] string __SegmentEndMarker { get => "END OF SEGMENT"; set { } }
        
        [JsonIgnore] double? _totalDuration;
        [JsonIgnore]public double TotalDuration =>Frames.IsEmpty()?0f: _totalDuration??=Frames.Select(f => f.DeltaTime).Sum();
    }

    [JsonProperty] public Segment Begin { get; init; }
    [JsonProperty] public Segment Loop { get; init; }
    [JsonProperty] public Segment End { get; init; }
    [JsonProperty] string _Record_EndMarker { get => "END OF RECORD"; set { } }
    [System.Serializable] public struct Frame
    {
        [field: SerializeField] public double DeltaTime { get; init; }
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
