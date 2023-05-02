using MarkusSecundus.PhysicsSwordfight.Sword;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using MarkusSecundus.Utils.Datastructs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Recording
{
    /// <summary>
    /// Object describing sequence of calls to <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/> happening in a time frame.
    /// </summary>
    [System.Serializable]
    public class SwordMovementRecord
    {
        /// <summary>
        /// Sequence of recorded calls to <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/>
        /// </summary>
        [System.Serializable]
        public class Track
        {
            /// <summary>
            /// List of frames, sorted by their time of invocation
            /// </summary>
            [JsonProperty] public Frame[] Frames { get; init; }
            /// <summary>
            /// Marker to ease orientation in the file
            /// </summary>
            [JsonProperty] string __SegmentEndMarker { get => "END OF SEGMENT"; set { } }

            [JsonIgnore] double? _totalDuration;
            /// <summary>
            /// Total duration of the whole track
            /// </summary>
            [JsonIgnore] public double TotalDuration => Frames.IsNullOrEmpty() ? 0f : _totalDuration ??= Frames.Select(f => f.DeltaTime).Sum();
        }
        /// <summary>
        /// Track of commands that makes up body of the recording 
        /// </summary>
        [JsonProperty] public Track Loop { get; init; }
        /// <summary>
        /// Marker to ease orientation in the file
        /// </summary>
        [JsonProperty] string _Record_EndMarker { get => "END OF RECORD"; set { } }

        /// <summary>
        /// Object describing one concrete call of <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/>
        /// </summary>
        [System.Serializable]
        public struct Frame
        {
            /// <summary>
            /// Time elapsed from the previous <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/> call
            /// </summary>
            [field: SerializeField] public double DeltaTime { get; init; }
            /// <summary>
            /// Value of the command
            /// </summary>
            [field: SerializeField] public Command Value { get; init; }

        }

        /// <summary>
        /// Equivalent to <see cref="ISwordMovement.MovementCommand"/> that is serializable by <see cref="Newtonsoft.Json"/> and uses coordinates relative to the swordsman's transform.
        /// </summary>
        [System.Serializable]
        public struct Command
        {
            /// <summary>
            /// Point corresponding to <c><see cref="ISwordMovement.MovementCommand.AnchorPoint"/> + <see cref="ISwordMovement.MovementCommand.LookDirection"/></c> but in swordsman's transform's local space
            /// </summary>
            [JsonProperty] public SerializableVector3 LookPoint { get; set; }
            /// <summary>
            /// Same as <see cref="ISwordMovement.MovementCommand.AnchorPoint"/> but in swordsman's transform's local space
            /// </summary>
            [JsonProperty] public SerializableVector3 AnchorPoint { get; set; }
            /// <summary>
            /// Point corresponding to <c><see cref="ISwordMovement.MovementCommand.AnchorPoint"/> + <see cref="ISwordMovement.MovementCommand.UpDirection"/></c> but in swordsman's transform's local space
            /// </summary>
            [JsonProperty] public SerializableVector3? UpPoint { get; set; }

            /// <summary>
            /// Convert <see cref="ISwordMovement.MovementCommand"/> that has values in world space to <see cref="Command"/> with values relative to provided transform.
            /// </summary>
            /// <param name="c">Command to convert</param>
            /// <param name="relativeTo">Transform defining the local space</param>
            /// <returns>Serializable equivalent of provided command</returns>
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
            /// <summary>
            /// Convert <see cref="Command"/> with values relative to provided transform into worldspace <see cref="ISwordMovement.MovementCommand"/>.
            /// </summary>
            /// <param name="relativeTo">Transform defining the local space</param>
            /// <returns>Corresponding command for <see cref="ISwordMovement.MoveSword(ISwordMovement.MovementCommand)"/></returns>
            public ISwordMovement.MovementCommand ToCommand(Transform relativeTo)
            {
                LookPoint = relativeTo.LocalToGlobal(LookPoint);
                AnchorPoint = relativeTo.LocalToGlobal(AnchorPoint);
                if (UpPoint != null) UpPoint = relativeTo.LocalToGlobal(UpPoint.Value);

                return new ISwordMovement.MovementCommand
                {
                    LookDirection = (Vector3)LookPoint - AnchorPoint,
                    AnchorPoint = AnchorPoint,
                    UpDirection = UpPoint == null ? null : (Vector3)UpPoint - (Vector3)AnchorPoint,
                    HoldingForce = 0f //TODO: add as a field to the recording when I'm sure I want to be doing it this way
                };
            }
        }
    }
}