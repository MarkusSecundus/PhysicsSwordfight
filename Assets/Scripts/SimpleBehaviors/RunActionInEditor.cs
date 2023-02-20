using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class RunActionInEditor : MonoBehaviour
{
    public enum ShouldRun
    {
        No=default, Yes=454313
    }

    public UnityEvent ToRun;

    public ShouldRun RunButton = ShouldRun.No;

    private void Update()
    {
        
        try
        {
            if (RunButton==ShouldRun.Yes)
            {
                Debug.Log($"Running an action: '{name}', value: '{RunButton}'");
                RunButton = ShouldRun.No;
                ToRun?.Invoke();
            }
        }
        finally
        {
            RunButton = ShouldRun.No;
        }
    }
}
