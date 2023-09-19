using MarkusSecundus.MultiInput;
using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight
{
    [DefaultExecutionOrder(-490)]
    public class SpecificDeviceInput : MonoBehaviour, ISwordInput
    {   
        [System.Serializable]
        public struct AxisDefinition
        {
            public int WindowSize;
            public KeyCode Positive, Negative;
            public int MouseAxis;
        }
        private class AxisTracker
        {
            AxisDefinition def;
            Queue<(float Value, float Delta)> window;
            public float Value { get; private set; }
            public float Raw { get; private set; }

            bool isMouseMovementAxis => def.MouseAxis > 0;
            public AxisTracker(AxisDefinition def)
            {
                this.def = def;
                if (isMouseMovementAxis) return;
                window = new(def.WindowSize);
            }
            public void Update(SpecificDeviceInput parent)
            {
                if (isMouseMovementAxis)
                {
                    var mouse = parent.mouse;
                    Value = mouse.IsNil()?0f:mouse.Axes[def.MouseAxis - 1];
                    Raw = mouse.IsNil() ? 0f : mouse.AxesRaw[def.MouseAxis - 1];
                    return;
                }

                float newValue = 0;
                if (parent.GetKey(def.Positive)) newValue += 1;
                if (parent.GetKey(def.Negative)) newValue -= 1;
                if(window.Count >= def.WindowSize) window.Dequeue();
                window.Enqueue((newValue, Time.deltaTime));
                float valueSum = 0f, deltaSum = 0f;
                foreach(var (v, d) in window)
                {
                    valueSum += v * d;
                    deltaSum += d;
                }
                Value = valueSum / deltaSum;
                Raw = newValue;
            }
        }

        [SerializeField] public int MouseIndex;
        [SerializeField] public int KeyboardIndex;

        [SerializeField] SerializableDictionary<InputAxis, AxisDefinition> Axes;
        Dictionary<InputAxis, AxisTracker> axes = new();

        void Start()
        {
            foreach (var (axis, def) in Axes.Values) axes[axis] = new AxisTracker(def);
        }
        void Update()
        {
            foreach (var (_, tracker) in axes) tracker.Update(this);
        }

        IMouse _mouse = null;
        IKeyboard _keyboard = null;
        IMouse mouse => _mouse.IsNotNil()
                        ? _mouse
                        : (MouseIndex < IInputProvider.Instance.ActiveMice.Count)
                            ? _mouse = IInputProvider.Instance.ActiveMice.ElementAt(MouseIndex)
                            : null
                        ;
        IKeyboard keyboard => _keyboard.IsNotNil()
                        ? _keyboard
                        : (KeyboardIndex < IInputProvider.Instance.ActiveKeyboards.Count)
                            ? _keyboard = IInputProvider.Instance.ActiveKeyboards.ElementAt(KeyboardIndex)
                            : null
                        ;

        public float GetAxis(InputAxis axis) => axes?.TryGetValue(axis, out var a)==true ? a.Value : 0f;

        public float GetAxisRaw(InputAxis axis) => axes?.TryGetValue(axis, out var a)==true ? a.Raw : 0f;

        public Ray? GetInputRay() => mouse?.WorldPositionRay;

        public bool GetKey(KeyCode code) => (keyboard?.GetButton(code)==true) || (mouse?.GetButton(code.AsMouseKeyCode()) == true);

        public bool GetKeyDown(KeyCode code) => (keyboard?.GetButtonDown(code)==true) || (mouse?.GetButtonDown(code.AsMouseKeyCode())==true);

        public bool GetKeyUp(KeyCode code) => (keyboard?.GetButtonUp(code)==true) || (mouse?.GetButtonUp(code.AsMouseKeyCode())==true);
    }

}
