using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class RunActionInEditor : MonoBehaviour
{
    public UnityEvent ToRun;


#if UNITY_EDITOR
    [CustomEditor(typeof(RunActionInEditor))]
    public class Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Run"))
                ((RunActionInEditor)target).ToRun?.Invoke();
        }
    }
#endif
}

