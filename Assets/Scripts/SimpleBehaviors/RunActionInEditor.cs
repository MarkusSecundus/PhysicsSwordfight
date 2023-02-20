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
                RunButton = false;
                Debug.Log($"Running an action: '{name}'");
                ToRun?.Invoke();
            }
        }
        finally
        {
            RunButton = false;
        }
    }
}
