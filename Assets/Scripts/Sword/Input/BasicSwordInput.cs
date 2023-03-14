using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSwordInput : ISwordInput
{
    public bool IsDisabled = false;
    public KeyCode DisableKey = KeyCode.Escape;

    public Camera inputCamera;
    public void Start()
    {
        //Cursor.lockState = CursorLockMode.;
        inputCamera ??= Camera.main ?? throw new NullReferenceException("No camera found");
    }


     
    public override Ray? GetInputRay()
        =>IsDisabled?null: inputCamera.ScreenPointToRay(Input.mousePosition);

    public override bool GetKey(KeyCode code)
        =>IsDisabled?false: Input.GetKey(code);

    public override bool GetKeyDown(KeyCode code)
        => IsDisabled ? false : Input.GetKeyDown(code);

    public override bool GetKeyUp(KeyCode code)
        => IsDisabled ? false : Input.GetKeyUp(code);

    public override float GetAxis(InputAxis axis)
        => IsDisabled ? 0f : Input.GetAxis(axis.Name());
    public override float GetAxisRaw(InputAxis axis)
        => IsDisabled ? 0f : Input.GetAxisRaw(axis.Name());


    private void Update()
    {
        if (Input.GetKeyDown(DisableKey)) IsDisabled = !IsDisabled;
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
