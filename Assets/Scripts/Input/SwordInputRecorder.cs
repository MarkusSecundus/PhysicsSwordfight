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
    /// <summary>
    /// Decorator for <see cref="ISwordInput"/> that records its values and reports finished reporting via an event.
    /// </summary>
    public class SwordInputRecorder : MonoBehaviour, ISwordInput
    {
        /// <summary>
        /// Input provider to be recorded. Must implement <see cref="ISwordInput"/>.
        /// </summary>
        public Component InputToRecord;
        ISwordInput inputToRecord => (ISwordInput)InputToRecord;

        /// <summary>
        /// Pressing this key starts the recording
        /// </summary>
        public KeyCode StartRecordingKey = KeyCode.F7;
        /// <summary>
        /// Pressing this key finishes the recording and invoke <see cref="OnRecordFinished"/> event with it.
        /// </summary>
        public KeyCode FinishRecordingKey = KeyCode.F8;

        /// <summary>
        /// Callback to call on each record when its recording is finished.
        /// </summary>
        public UnityEvent<List<Frame>> OnRecordFinished;

        private HashSet<InputAxis> axesToRecord = new HashSet<InputAxis>();

        /// <summary>
        /// Captured state of a single recorded frame.
        /// </summary>
        [System.Serializable]
        public struct Frame
        {
            /// <summary>
            /// Set of all keys pressed at given moment
            /// </summary>
            [SerializeField]
            public HashSet<KeyCode> KeysPressed;
            /// <summary>
            /// Ray representing projection of the mouse cursor.
            /// In coordinates relative to the recorder's transform!
            /// </summary>
            [SerializeField]
            public SerializableRay? CursorRay;

            /// <summary>
            /// List of axis values
            /// </summary>
            [SerializeField]
            public Dictionary<InputAxis, float> Axes;
            /// <summary>
            /// List of raw axis values
            /// </summary>
            [SerializeField]
            public Dictionary<InputAxis, float> AxesRaw;
        }

        private List<Frame> currentRecording;
        private bool isRecording => currentRecording != null;

        private void Update()
        {
            if (inputToRecord.GetKeyDown(FinishRecordingKey)) FinishRecording();
            if (inputToRecord.GetKeyDown(StartRecordingKey)) StartRecording();
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
            OnRecordFinished?.Invoke(recording);
        }

        /// <inheritdoc/>
        public bool GetKey(KeyCode code) => inputToRecord.GetKey(code);

        /// <inheritdoc/>
        public bool GetKeyUp(KeyCode code) => inputToRecord.GetKeyUp(code);

        /// <inheritdoc/>
        public bool GetKeyDown(KeyCode code) => inputToRecord.GetKeyDown(code);

        /// <inheritdoc/>
        public float GetAxis(InputAxis axis)
        {
            float ret() => inputToRecord.GetAxis(axis);
            if (axesToRecord.Add(axis))
            {
                if (currentRecording.TryPeek(out var last)) last.Axes[axis] = ret();
            }
            return ret();
        }
        /// <inheritdoc/>
        public float GetAxisRaw(InputAxis axis)
        {
            float ret() => inputToRecord.GetAxisRaw(axis);
            if (axesToRecord.Add(axis))
            {
                if (currentRecording.TryPeek(out var last)) last.AxesRaw[axis] = ret();
            }
            return ret();
        }

        /// <inheritdoc/>
        public Ray? GetInputRay() => inputToRecord.GetInputRay();
    }

}