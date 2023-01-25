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
    public ISwordInput inputToRecord;

    public KeyCode startRecordingKey = KeyCode.F7, finishRecordingKey = KeyCode.F8;

    public UnityEvent<List<Frame>> onRecordFinished;

    [System.Serializable]
    public struct Frame
    {
        /// <summary>
        /// Set of all keys pressed at given moment
        /// </summary>
        public HashSet<KeyCode> KeysPressed;
        /// <summary>
        /// In coordinates relative to the recorder's transform!
        /// Ray representing projection of the mouse cursor.
        /// </summary>
        public SerializableRay? CursorRay;
    }

    private List<Frame> currentRecording;
    private bool isRecording => currentRecording != null;

    private void Update() 
    {
        if (inputToRecord.GetKeyDown(finishRecordingKey)) FinishRecording();
        if (inputToRecord.GetKeyDown(startRecordingKey)) StartRecording();
    }

    private void FixedUpdate()
    {
        if (!isRecording) return;

        var frame = new Frame { KeysPressed = new HashSet<KeyCode>() };
        foreach(var key in EnumUtil.GetValues<KeyCode>())
        {
            if(Input.GetKey(key)) frame.KeysPressed.Add(key);
        }

        var ray = inputToRecord.GetInputRay();
        frame.CursorRay = ray == null ? (Ray?)null : transform.GlobalToLocal(ray.Value);
        Debug.Log($"local ray: {(Ray)frame.CursorRay}");

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
        currentRecording = new List<Frame>();
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

