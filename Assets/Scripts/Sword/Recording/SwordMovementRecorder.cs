using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.Submodules;
using MarkusSecundus.PhysicsSwordfight.Sword;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Recording
{
    /// <summary>
    /// Utility class for recording sword movement commands into JSON files.
    /// 
    /// <para>
    /// Creates an instance of <see cref="SwordMovementMode_Recording"/> that gets injected into target <see cref="SwordMovement"/>.
    /// </para>
    /// </summary>
    public class SwordMovementRecorder : MonoBehaviour
    {
        /// <summary>
        /// <see cref="SwordMovement"/> to record
        /// </summary>
        [SerializeField] SwordMovement Target;
        /// <summary>
        /// Key to start recording
        /// </summary>
        public KeyCode BeginRecordKey = KeyCode.F6;
        /// <summary>
        /// Key to finish recording and save the result track to a file
        /// </summary>
        public KeyCode EndRecordKey = KeyCode.F7;

        /// <summary>
        /// Instructions for saving finished tracks to filesystem
        /// </summary>
        [System.Serializable]
        public class ResultFilePathDescriptor
        {
            /// <summary>
            /// Path format for the recording files, containing <c>{0}</c> as placeholder for the file index.
            /// </summary>
            public string Format = "/data/{0}.json";
            /// <summary>
            /// Index for the next file to be saved
            /// </summary>
            public int NextIndex = 1;
            /// <summary>
            /// Move index by one and get corresponding full path
            /// </summary>
            /// <returns>Full path for the next file to be recorded</returns>
            public string GetNextPath() => string.Format(Format, NextIndex++);
        }
        public ResultFilePathDescriptor ResultDestination;

        ISwordInput Input => Target.Input;
        Transform SwordWielder => Target.SwordWielder;

        void Start()
        {
            Target = Target.IfNil(GetComponentInChildren<SwordMovement>());
            SetUpRecording();
        }

        void SetUpRecording()
        {
            var recorderModule = new SwordMovementMode_Recording() { Modes = Target.Modes };
            recorderModule.Init(Target);
            recorderModule.MovementReporter.AddListener(DoRecord);
            Target.Modes = new ScriptSubmodulesContainer<KeyCode, SwordMovement.Module, ISwordMovement> { Default = recorderModule, Values = new Dictionary<KeyCode, SwordMovement.Module>() };
        }

        List<SwordMovementRecord.Frame> currentFrame = null;
        bool isRecording => currentFrame != null;
        double beginTime;
        void Update()
        {
            if (!isRecording && Input.GetKeyDown(BeginRecordKey))
                startRecording();
            if (isRecording && Input.GetKeyDown(EndRecordKey))
                finishRecording();

            void startRecording()
            {
                if (isRecording) return;
                currentFrame = new();
                beginTime = Time.timeAsDouble;
            }
            void finishRecording()
            {
                if (!isRecording) return;
                var finishedRecord = new SwordMovementRecord { Loop = new SwordMovementRecord.Track { Frames = currentFrame.ToArray() } };
                currentFrame = null;
                Debug.Log($"Finished recording - new record is {finishedRecord.Loop.Frames.Length} long", this);
                saveRecord(finishedRecord);
            }
            void saveRecord(SwordMovementRecord toSave)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(toSave, Newtonsoft.Json.Formatting.Indented);

                var path = ResultDestination.GetNextPath();
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, json);

                Debug.Log($"Saved record to file '{path}'", this);
            }
        }


        void DoRecord(ISwordMovement.MovementCommand c)
        {
            if (isRecording)
            {
                var currentTime = Time.timeAsDouble;
                currentFrame.Add(new SwordMovementRecord.Frame { DeltaTime = currentTime - beginTime, Value = SwordMovementRecord.Command.Make(c, SwordWielder) });
                beginTime = currentTime;
            }
        }
    }
}