using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMovementRecorder : MonoBehaviour
{
    [SerializeField] SwordMovement Target;

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

    void DoRecord(ISwordMovement.MovementCommand c)
    {
        Debug.Log($"fr.{Time.frameCount}: {c}");
    }
}
