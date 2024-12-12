using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input
{
    /// <summary>
    /// Basic implementation of <see cref="ISwordInput"/> that reads input from <see cref="UnityEngine.Input"/>.
    /// </summary>
    public class BasicSwordInput : MonoBehaviour, ISwordInput
    {
        bool isDisabled => !this.enabled;

        /// <summary>
        /// Camera to be used for shooting rays in <see cref="GetInputRay"/>
        /// </summary>
        [Tooltip("Camera to be used for shooting rays in GetInputRay()")]
        public Camera InputCamera;
        void Start()
        {
            InputCamera ??= Camera.main ?? throw new NullReferenceException("No camera found");
        }

        [System.Serializable] struct AdvancedConfig
        {
            [SerializeField] public InputAxis SwordJoystickXAxis;
            [SerializeField] public InputAxis SwordJoystickYAxis;
            [SerializeField] public SerializableDictionary<KeyCode, KeyCode[]> _keyRemappings;
            [SerializeField] public SerializableDictionary<InputAxis, InputAxis[]> _axisRemappings;
        }
        [SerializeField] AdvancedConfig _advanced;

        /// <inheritdoc/>
        public Ray? GetInputRay()
            => isDisabled ? null : InputCamera.ScreenPointToRay(_getMousePos());


        private bool _getKey(KeyCode code, Func<KeyCode, bool> func)
        {
            if (_advanced._keyRemappings.Values.TryGetValue(code, out var codes))
                return codes.Any(func);
            return func(code);
        }
        
        private float _getAxis(InputAxis axis, Func<InputAxis, float> func)
        {
            if (_advanced._axisRemappings.Values.TryGetValue(axis, out var axes))
                return Mathf.Clamp(axes.Select(func).Sum(), -1f, 1f);
            return func(axis);
        }

        bool _wasJoystickUsed = false;
        private Vector3 _getMousePos()
        {
            var fromJoystick = new Vector2(UnityEngine.Input.GetAxis(_advanced.SwordJoystickXAxis.ToString()), UnityEngine.Input.GetAxis(_advanced.SwordJoystickYAxis.ToString()));
            if(_wasJoystickUsed || fromJoystick != Vector2.zero)
            {
                _wasJoystickUsed = true;
                fromJoystick = (fromJoystick + Vector2.one) * 0.5f; // normalize from <-1;+1> range to <0;1>
                var rect = InputCamera.pixelRect;
                var ret = new Vector2(Mathf.Lerp(rect.xMin, rect.xMax, fromJoystick.x), Mathf.Lerp(rect.yMax, rect.yMin, fromJoystick.y));
                return ret;
            }
            return UnityEngine.Input.mousePosition;
        }

        /// <inheritdoc/>
        public bool GetKey(KeyCode code)
            => isDisabled ? false : _getKey(code, UnityEngine.Input.GetKey);

        /// <inheritdoc/>
        public bool GetKeyDown(KeyCode code)
            => isDisabled ? false : _getKey(code, UnityEngine.Input.GetKeyDown);

        /// <inheritdoc/>
        public bool GetKeyUp(KeyCode code)
            => isDisabled ? false : _getKey(code, UnityEngine.Input.GetKeyUp);

        /// <inheritdoc/>
        public float GetAxis(InputAxis axis)
            => isDisabled ? 0f : _getAxis(axis, a=>UnityEngine.Input.GetAxis(a.ToString()));
        /// <inheritdoc/>
        public float GetAxisRaw(InputAxis axis)
            => isDisabled ? 0f : _getAxis(axis, a=>UnityEngine.Input.GetAxisRaw(a.ToString()));



#if false
    private void OnDrawGizmos()
    {
        var ray = GetInputRay();
        if(ray != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.Value);
        }
    }
#endif
    }
}