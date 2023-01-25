using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class SwordInputRecorder : MonoBehaviour
{
    public ISwordInput baseInput;

    public KeyCode startRecordingKey = KeyCode.F7, finishRecordingKey = KeyCode.F8;

    public UnityEvent<List<FrameImage>> onRecordFinished;

    [System.Serializable]
    public struct FrameImage
    {
        public HashSet<KeyCode> Keys;
        public SerializableRay? InputRayLocal;
    }

    private List<FrameImage> currentRecording;
    private bool isRecording => currentRecording != null;

    private void Update() 
    {
        if (baseInput.GetKeyDown(finishRecordingKey)) FinishRecording();
        if (baseInput.GetKeyDown(startRecordingKey)) StartRecording();
    }

    private void FixedUpdate()
    {
        if (!isRecording) return;

        var frame = new FrameImage { Keys = new HashSet<KeyCode>() };
        foreach(var key in EnumUtil.GetValues<KeyCode>())
        {
            if(Input.GetKey(key)) frame.Keys.Add(key);
        }

        var ray = baseInput.GetInputRay();
        frame.InputRayLocal = ray == null ? (Ray?)null : transform.GlobalToLocal(ray.Value);
        Debug.Log($"local ray: {(Ray)frame.InputRayLocal}");

        currentRecording.Add(frame);
    }


    private double timeStamp;
    private void StartRecording()
    {
        if (isRecording)
        {
            Debug.Log("Already recording! Finish first to start another recording");
            return;
        }
        timeStamp = Time.timeAsDouble;
        Debug.Log("Starting recording!");
        currentRecording = new List<FrameImage>();
    }
    private void FinishRecording()
    {
        if (!isRecording)
        {
            Debug.Log("Cannot finish recording when none is happening!");
            return;
        }
        Debug.Log($"Recording finished! Number of frames recorded: {currentRecording.Count}. Duration: {Time.timeAsDouble - timeStamp}");
        var recording = currentRecording;
        currentRecording = null;
        onRecordFinished?.Invoke(recording);
    }
}

