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
        public void StartKey(KeyCode code)
        {
            keys[code] = new KeyState { BeginFrame = CurrentFrame, EndedFrame = null };
        }
        public void EndKey(KeyCode code)
        {
            if (keys.TryGetValue(code, out var current))
            {
                current.EndedFrame = CurrentFrame;
                keys[code] = current;
            }
        }

        public void SetAxisValue(InputAxis axis, float value)
        {
            value = ClampAxis(value);
            axes[axis] = new AxisState { Value = value };
        }

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


        public float GetAxis(InputAxis axis) => axes.TryGetValue(axis, out var ret) ? ret.Value : 0f;

        public float GetAxisRaw(InputAxis axis) => axes.TryGetValue(axis, out var ret) ? Mathf.Sign(ret.Value) : 0f;

        public Ray? GetInputRay() => inputRayValue;

        public bool GetKey(KeyCode code) => keys.TryGetValue(code, out var state) && state.EndedFrame == null || state.EndedFrame >= CurrentFrame;

        public bool GetKeyDown(KeyCode code) => keys.TryGetValue(code, out var state) && state.BeginFrame == CurrentFrame;
        public bool GetKeyUp(KeyCode code) => keys.TryGetValue(code, out var state) && state.EndedFrame == CurrentFrame;
    }
}