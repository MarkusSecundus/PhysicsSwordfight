using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSwordInput : ISwordInput
{
    public Camera inputCamera;
    public void Start()
    {
        inputCamera ??= Camera.main ?? throw new NullReferenceException("No camera found");
    }


     
    public override Ray? GetInputRay()
        => inputCamera.ScreenPointToRay(Input.mousePosition);

    public override bool GetKey(KeyCode code)
        => Input.GetKey(code);

    public override bool GetKeyDown(KeyCode code)
        => Input.GetKeyDown(code);

    public override bool GetKeyUp(KeyCode code)
        => Input.GetKeyUp(code);

    public override float GetAxis(string axisName)
        => Input.GetAxis(axisName);
}
