using System;
using System.Collections;
using System.Collections.Generic;
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
        public Camera InputCamera;
        void Start()
        {
            InputCamera ??= Camera.main ?? throw new NullReferenceException("No camera found");
        }


        /// <inheritdoc/>
        public Ray? GetInputRay()
            => isDisabled ? null : InputCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);

        /// <inheritdoc/>
        public bool GetKey(KeyCode code)
            => isDisabled ? false : UnityEngine.Input.GetKey(code);

        /// <inheritdoc/>
        public bool GetKeyDown(KeyCode code)
            => isDisabled ? false : UnityEngine.Input.GetKeyDown(code);

        /// <inheritdoc/>
        public bool GetKeyUp(KeyCode code)
            => isDisabled ? false : UnityEngine.Input.GetKeyUp(code);

        /// <inheritdoc/>
        public float GetAxis(InputAxis axis)
            => isDisabled ? 0f : UnityEngine.Input.GetAxis(axis.ToString());
        /// <inheritdoc/>
        public float GetAxisRaw(InputAxis axis)
            => isDisabled ? 0f : UnityEngine.Input.GetAxisRaw(axis.ToString());



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