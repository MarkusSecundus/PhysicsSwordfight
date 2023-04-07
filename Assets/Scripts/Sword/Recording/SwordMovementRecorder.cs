

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SwordMovementRecorder : MonoBehaviour
{
    [SerializeField] SwordMovement Target;
    public KeyCode BeginRecordKey = KeyCode.F6, 
                     EndRecordKey = KeyCode.F7,
                       SaveAllKey = KeyCode.F1;
    public string ResultPath = "data.json";
    ISwordInput Input => Target.Input;
    Transform SwordWielder => Target.Joint.connectedBody.transform;

    private void Start()
    {
        Target = Target.IfNil(GetComponentInChildren<SwordMovement>());
        SetUpRecording();
    }

    void SetUpRecording()
    {
        var recorderModule = new SwordMovementMode_Recording() { Modes = Target.Modes};
        recorderModule.Init(Target);
        recorderModule.MovementReporter.AddListener(DoRecord);
        Target.Modes = new ScriptSubmodulesContainer<KeyCode, SwordMovement.Submodule, ISwordMovement> { Default = recorderModule, Values = new Dictionary<KeyCode, SwordMovement.Submodule>() };
    }

    public List<SwordMovementRecord> records { get; } = new List<SwordMovementRecord>();
    List<SwordMovementRecord.Frame> currentFrame = null;
    bool isRecording => currentFrame != null;
    double beginTime;
    private void Update()
    {
        if (!isRecording && Input.GetKeyDown(BeginRecordKey))
            startRecording();
        if (isRecording && Input.GetKeyDown(EndRecordKey))
            finishRecording();
        if (Input.GetKeyDown(SaveAllKey))
        {
            finishRecording();
            saveAll();
        }

        void startRecording()
        {
            if (isRecording) return;
            currentFrame = new();
            beginTime = Time.timeAsDouble;
            Debug.Log($"Started recording - current record count is {records.Count}", this);
        }
        void finishRecording()
        {
            if (!isRecording) return;
            records.Add(new SwordMovementRecord { Loop = new SwordMovementRecord.Segment { Frames = currentFrame.ToArray() } });
            currentFrame = null;
            Debug.Log($"Finished recording - current record count is {records.Count}, added record is {records[^1].Loop.Frames.Length} long", this);
        }
        void saveAll()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(records, Newtonsoft.Json.Formatting.Indented);

            var dir = Path.GetDirectoryName(ResultPath);
            if(!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(Path.GetDirectoryName(ResultPath));
            File.WriteAllText(ResultPath, json);

            Debug.Log($"Saved records to file '{ResultPath}'", this);
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
