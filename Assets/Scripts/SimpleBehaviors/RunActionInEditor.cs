using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class RunActionInEditor : MonoBehaviour
{

    public UnityEvent ToRun;

    public bool RunButton = false;

    private void Start() => RunButton = false;
    private void OnEnable() => RunButton = false;
    private void OnBecameVisible() => RunButton = false;
    private void OnApplicationFocus(bool focus) => RunButton = false;

    private void Update()
    {
        
        try
        {
            if (RunButton==true)
            {
                var buttonValue = RunButton;
                RunButton = false;
                Debug.Log($"Running an action: '{name}', value: '{buttonValue}'");
                ToRun?.Invoke();
            }
        }
        finally
        {
            RunButton = false;
        }
    }
}
