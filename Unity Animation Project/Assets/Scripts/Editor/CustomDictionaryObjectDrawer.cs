using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

[CustomPropertyDrawer(typeof(TrickTheEditorParent), true)]
public class CustomDictionaryObjectDrawer : PropertyDrawer
{
    private float positionValue = 60.0f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        int initialIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect keyRect = new Rect(position.x, position.y, position.width - positionValue, position.height);
        Rect valueRect = new Rect(position.x + position.width - positionValue, position.y, positionValue, position.height);

        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"), GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

        EditorGUI.indentLevel = initialIndent;

        EditorGUI.EndProperty();
    }
}
