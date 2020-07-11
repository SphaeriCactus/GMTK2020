using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorGUILayoutExt
    {
    public static GUIStyle miniHeaderStyle => EditorStyles.boldLabel;

    public static void AddSpacer()
        {
        Rect rect = EditorGUILayout.GetControlRect (false, 1);
        rect.height = 1;

        EditorGUI.DrawRect (rect, new Color (0.5f, 0.5f, 0.5f, 1));
        }

    public static void GetAddProperty(SerializedProperty property)
        {
        if (property != null)
            EditorGUILayout.PropertyField (property);
        }
    }
