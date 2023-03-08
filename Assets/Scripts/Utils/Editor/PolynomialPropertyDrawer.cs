using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Polynomial))]
public class PolynomialPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Polynomial p = default;
        const float segmentWidth = 80;

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        fld(0, nameof(p.x3), "x^3 +");
        fld(1, nameof(p.x2), "x^2 +");
        fld(2, nameof(p.x1), "x +");
        fld(3, nameof(p.c), "c");

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();

        void fld(int index, string name, string text)
        {
            var prop = new Rect(position.x + segmentWidth * index, position.y, segmentWidth/2, position.height);
            var lbl = new Rect(position.x + segmentWidth * index + segmentWidth/2, position.y, segmentWidth/2, position.height);
            EditorGUI.PropertyField(prop, property.FindPropertyRelative(name), GUIContent.none);
            EditorGUI.LabelField(lbl, text);
        }
    }
}
