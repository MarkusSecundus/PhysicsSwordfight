using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    /// <summary>
    /// Simple container for <see cref="UnityEvent"/> with additional editor button for running the event
    /// </summary>
    [ExecuteInEditMode]
    public class RunAction : MonoBehaviour
    {
        /// <summary>
        /// Doccumentation text string
        /// </summary>
        public string Description;
        /// <summary>
        /// The action to run when pressing Run button
        /// </summary>
        public UnityEvent ToRun;

        /// <summary>
        /// Invoke the Action
        /// </summary>
        public void Invoke() => ToRun.Invoke();

        /// <summary>
        /// Put this action inside another action as its member
        /// </summary>
        /// <param name="parent">The action to nest into</param>
        public void NestInto(RunAction parent) => parent.ToRun.AddListener(this.Invoke);

#if UNITY_EDITOR
        /// <summary>
        /// Editor drawer responsible for drawing the <see cref="RunAction"/>'s Run button
        /// </summary>
        [CustomEditor(typeof(RunAction))]
        public class Inspector : Editor
        {
            /// Draws the default inspector and then a Run button
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