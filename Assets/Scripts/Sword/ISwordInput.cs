using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ISwordInput : MonoBehaviour
{
    public abstract bool GetKey(KeyCode code);
    public abstract bool GetKeyUp(KeyCode code);
    public abstract bool GetKeyDown(KeyCode code);

    public abstract float GetAxis(string axisName);

    public abstract Ray? GetInputRay();
}
