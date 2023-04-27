using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input
{

    public class BasicSwordInput : MonoBehaviour, ISwordInput
    {

        public bool IsDisabled = false;
        bool isDisabled => this.IsDisabled || !this.enabled;
        public KeyCode DisableKey = KeyCode.Escape;

        public Camera inputCamera;
        public void Start()
        {
            inputCamera ??= Camera.main ?? throw new NullReferenceException("No camera found");
        }



        public Ray? GetInputRay()
            => isDisabled ? null : inputCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);

        public bool GetKey(KeyCode code)
            => isDisabled ? false : UnityEngine.Input.GetKey(code);

        public bool GetKeyDown(KeyCode code)
            => isDisabled ? false : UnityEngine.Input.GetKeyDown(code);

        public bool GetKeyUp(KeyCode code)
            => isDisabled ? false : UnityEngine.Input.GetKeyUp(code);

        public float GetAxis(InputAxis axis)
            => isDisabled ? 0f : UnityEngine.Input.GetAxis(axis.Name());
        public float GetAxisRaw(InputAxis axis)
            => isDisabled ? 0f : UnityEngine.Input.GetAxisRaw(axis.Name());


        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(DisableKey)) IsDisabled = !IsDisabled;
        }
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