using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullSwordInput : ISwordInput
{
    public override float GetAxis(InputAxis axis) => 0f;
    public override Ray? GetInputRay() => null;
    public override bool GetKey(KeyCode code) => false;
    public override bool GetKeyDown(KeyCode code) => false;
    public override bool GetKeyUp(KeyCode code) => false;
}
