using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using MarkusSecundus.Utils;
using MarkusSecundus.Utils.Datastructs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MarkusSecundus.PhysicsSwordfight.Input
{

    public class SwordInputRecorder : MonoBehaviour, ISwordInput
    {
        public Component InputToRecord;
        ISwordInput inputToRecord => (ISwordInput)InputToRecord;

        public KeyCode startRecordingKey = KeyCode.F7, finishRecordingKey = KeyCode.F8;

        public UnityEvent<List<Frame>> onRecordFinished;

        private HashSet<InputAxis> axesToRecord = new HashSet<InputAxis>();

        [System.Serializable]
        public struct Frame
        {
            /// <summary>
            /// Set of all keys pressed at given moment
            /// </summary>
            [SerializeField]
            public HashSet<KeyCode> KeysPressed;
            /// <summary>
            /// In coordinates relative to the recorder's transform!
            /// Ray representing projection of the mouse cursor.
            /// </summary>
            [SerializeField]
            public SerializableRay? CursorRay;

            [SerializeField]
            public Dictionary<InputAxis, float> Axes;
            public Dictionary<InputAxis, float> AxesRaw;
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

            var frame = new Frame { KeysPressed = new HashSet<KeyCode>(), Axes = new Dictionary<InputAxis, float>(), AxesRaw = new Dictionary<InputAxis, float>() };
            foreach (var key in EnumHelpers.GetValues<KeyCode>())
            {
                if (UnityEngine.Input.GetKey(key)) frame.KeysPressed.Add(key);
            }
            foreach (var axis in axesToRecord)
            {
                var axisValue = inputToRecord.GetAxis(axis);
                if (axisValue != 0) frame.Axes[axis] = axisValue;

                axisValue = inputToRecord.GetAxisRaw(axis);
                if (axisValue != 0) frame.AxesRaw[axis] = axisValue;
            }


            var ray = inputToRecord.GetInputRay();
            frame.CursorRay = ray == null ? (Ray?)null : transform.GlobalToLocal(ray.Value);

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

        public bool GetKey(KeyCode code) => inputToRecord.GetKey(code);

        public bool GetKeyUp(KeyCode code) => inputToRecord.GetKeyUp(code);

        public bool GetKeyDown(KeyCode code) => inputToRecord.GetKeyDown(code);

        public float GetAxis(InputAxis axis)
        {
            float ret() => inputToRecord.GetAxis(axis);
            if (axesToRecord.Add(axis))
            {
                if (currentRecording.TryPeek(out var last)) last.Axes[axis] = ret();
            }
            return ret();
        }
        public float GetAxisRaw(InputAxis axis)
        {
            float ret() => inputToRecord.GetAxisRaw(axis);
            if (axesToRecord.Add(axis))
            {
                if (currentRecording.TryPeek(out var last)) last.AxesRaw[axis] = ret();
            }
            return ret();
        }

        public Ray? GetInputRay() => inputToRecord.GetInputRay();
    }

}