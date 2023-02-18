using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class RunActionInEditor : MonoBehaviour
{
    public UnityEvent ToRun;

    public bool RunButton = false;

    private void Update()
    {
        try
        {
            if (RunButton)
            {
                ToRun?.Invoke();
            }
        }
        finally
        {
            RunButton = false;
        }
    }
}
