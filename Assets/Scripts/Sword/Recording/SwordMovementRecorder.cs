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

    public class SwordMovementRecorder : MonoBehaviour
    {
        [SerializeField] SwordMovement Target;
        public KeyCode BeginRecordKey = KeyCode.F6,
                         EndRecordKey = KeyCode.F7;

        [System.Serializable]
        public class ResultFilePathDescriptor
        {
            public string Path = "/data/", Extension = ".json";
            public int NextIndex = 1;
            public string GetNextPath() => $"{Path}{NextIndex++}{Extension}";
        }
        public ResultFilePathDescriptor ResultDestination;

        ISwordInput Input => Target.Input;
        Transform SwordWielder => Target.Joint.connectedBody.transform;

        private void Start()
        {
            Target = Target.IfNil(GetComponentInChildren<SwordMovement>());
            SetUpRecording();
        }

        void SetUpRecording()
        {
            var recorderModule = new SwordMovementMode_Recording() { Modes = Target.Modes };
            recorderModule.Init(Target);
            recorderModule.MovementReporter.AddListener(DoRecord);
            Target.Modes = new ScriptSubmodulesContainer<KeyCode, SwordMovement.Submodule, ISwordMovement> { Default = recorderModule, Values = new Dictionary<KeyCode, SwordMovement.Submodule>() };
        }

        List<SwordMovementRecord.Frame> currentFrame = null;
        bool isRecording => currentFrame != null;
        double beginTime;
        private void Update()
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
                var finishedRecord = new SwordMovementRecord { Loop = new SwordMovementRecord.Segment { Frames = currentFrame.ToArray() } };
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