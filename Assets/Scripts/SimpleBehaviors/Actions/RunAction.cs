using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    [ExecuteInEditMode]
    public class RunAction : MonoBehaviour
    {
        public string Description;
        public UnityEvent ToRun;

        public void Invoke() => ToRun.Invoke();

        public void NestInto(RunAction parent) => parent.ToRun.AddListener(this.Invoke);

#if UNITY_EDITOR
        [CustomEditor(typeof(RunAction))]
        public class Inspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                if (GUILayout.Button("Run"))
                    ((RunAction)target)?.Invoke();
            }
        }
#endif
    }
}