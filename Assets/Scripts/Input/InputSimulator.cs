using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input
{

    public class InputSimulator : MonoBehaviour, ISwordInput
    {
        private static float ClampAxis(float axis) => Mathf.Clamp(axis, -1f, 1f);
        private static int CurrentFrame => Time.frameCount;

        /// <summary>
        /// Starts the press of the specified key.
        /// 
        /// <para>
        /// For the rest of the current frame, <see cref="GetKeyDown(KeyCode)"/> will be <c>true</c>.
        /// <see cref="GetKey(KeyCode)"/> will be <c>true</c> until <see cref="EndKey(KeyCode)"/> is called.
        /// If press of the specified key already is in progress, nothing will happen.
        /// </para>
        /// </summary>
        /// <param name="code">Key to start pressing</param>
        public void StartKey(KeyCode code)
        {
            keys[code] = new KeyState { BeginFrame = CurrentFrame, EndedFrame = null };
        }
        /// <summary>
        /// Ends the press of the specified key.
        /// 
        /// <para>
        /// For the rest of the current frame, <see cref="GetKeyUp(KeyCode)"/> will be <c>false</c>.
        /// Beginning the next frame, <see cref="GetKey(KeyCode)"/> will be <c>false</c></para>.
        /// If the specified key already is not pressed, nothing will happen.
        /// </summary>
        /// <param name="code">Key to end pressing</param>
        public void EndKey(KeyCode code)
        {
            if (keys.TryGetValue(code, out var current))
            {
                current.EndedFrame = CurrentFrame;
                keys[code] = current;
            }
        }

        /// <summary>
        /// Set value that will be returned by <see cref="GetAxis(InputAxis)"/>.
        /// 
        /// <para>
        /// <see cref="GetAxisRaw(InputAxis)"/> will return <see cref="Mathf.Sign(float)"/> of that value</para>
        /// </summary>
        /// <param name="axis">Axis to be set</param>
        /// <param name="value">Value to be set</param>
        public void SetAxisValue(InputAxis axis, float value)
        {
            value = ClampAxis(value);
            axes[axis] = new AxisState { Value = value };
        }

        /// <summary>
        /// Set value that will be returned by <see cref="GetInputRay"/>
        /// </summary>
        /// <param name="ray">Value of <see cref="GetInputRay"/></param>
        public void SetInputRay(Ray? ray)
        {
            inputRayValue = ray;
        }

        private struct KeyState
        {
            public int BeginFrame;
            public int? EndedFrame;
        }
        readonly Dictionary<KeyCode, KeyState> keys = new Dictionary<KeyCode, KeyState>();

        private struct AxisState
        {
            public float Value;
        }
        readonly Dictionary<InputAxis, AxisState> axes = new Dictionary<InputAxis, AxisState>();

        private Ray? inputRayValue = null;


        /// <inheritdoc/>
        public float GetAxis(InputAxis axis) => axes.TryGetValue(axis, out var ret) ? ret.Value : 0f;

        /// <inheritdoc/>
        public float GetAxisRaw(InputAxis axis) => axes.TryGetValue(axis, out var ret) ? Mathf.Sign(ret.Value) : 0f;

        /// <inheritdoc/>
        public Ray? GetInputRay() => inputRayValue;

        /// <inheritdoc/>
        public bool GetKey(KeyCode code) => keys.TryGetValue(code, out var state) && state.EndedFrame == null || state.EndedFrame >= CurrentFrame;

        /// <inheritdoc/>
        public bool GetKeyDown(KeyCode code) => keys.TryGetValue(code, out var state) && state.BeginFrame == CurrentFrame;
        /// <inheritdoc/>
        public bool GetKeyUp(KeyCode code) => keys.TryGetValue(code, out var state) && state.EndedFrame == CurrentFrame;
    }
}