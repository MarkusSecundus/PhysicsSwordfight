using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

public class PlaceOnCircle : EditorWindow
{
    public GameObject obj;
    public string passwd;
    public SerializedProperty prop;

    [MenuItem("Windos/PlaceOnCircle")]
    public static void ShowWindos()
    {
        EditorWindow.GetWindow(typeof(PlaceOnCircle));
    }


    private void OnEnable()
    {
        //serializedObject.FindProperty("CircleGizmo");
    }

    private void OnGUI()
    {
        GUI.Button(new Rect(25, 25, 100, 30), "Click!");
        //EditorGUILayout.PropertyField(prop);
    }
}
