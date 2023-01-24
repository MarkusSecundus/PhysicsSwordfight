using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;

public class SwordInputRecorder : MonoBehaviour
{
    public ISwordInput baseInput;

    public KeyCode startRecordingKey = KeyCode.F7;

    public UnityEvent<IReadOnlyList<FrameImage>> onRecordFinished;

    public struct FrameImage
    {
        public HashSet<KeyCode> Keys, KeysDown, KeysUp;
        public Ray? InputRayLocal;
    }

    private List<FrameImage> currentRecording;
    private bool isRecording => currentRecording != null;

    private void FixedUpdate()
    {
        if (baseInput.GetKeyUp(startRecordingKey)) FinishRecording();
        else if(baseInput.GetKeyDown(startRecordingKey)) StartRecording();

        if (!isRecording) return;

        var frame = new FrameImage { Keys = new HashSet<KeyCode>(), KeysDown = new HashSet<KeyCode>(), KeysUp = new HashSet<KeyCode>() };
        foreach(var key in EnumUtil.GetValues<KeyCode>())
        {
            if(Input.GetKeyDown(key)) frame.KeysDown.Add(key);
            if(Input.GetKeyUp(key)) frame.KeysUp.Add(key);
            if(Input.GetKey(key)) frame.Keys.Add(key);
        }

        var ray = baseInput.GetInputRay();
        frame.InputRayLocal = ray==null?(Ray?)null:transform.GlobalToLocal(ray.Value);

        currentRecording.Add(frame);
    }



    private void StartRecording() => currentRecording = new List<FrameImage>();
    private void FinishRecording()
    {
        var recording = currentRecording;
        currentRecording = null;
        onRecordFinished?.Invoke(recording);
    }
}
