using UnityEngine;
using UnityEditor;

public static class GUIExtension
    {
    private static GUIStyle style = new GUIStyle (EditorStyles.boldLabel);

    //public static EditorDrawProfile EditorProfile => AssetDatabase.LoadAssetAtPath ("Assets/Editor/_Settings/Design/Base Style.asset", typeof (EditorDrawProfile)) as EditorDrawProfile;

    public static void CreateLineSpacer()
        => EditorGUILayout.LabelField ("", GUI.skin.horizontalSlider); 

    public static void CreateLineSpacer(Rect rect, Color color, float height = 2)
        {
        rect.height = height;

        Color c = GUI.color;

        GUI.color = color;
            EditorGUI.DrawRect (rect, color);
        GUI.color = c;
        }

    public static void CreateNewSection(string str)
        {
        CreateLineSpacer ();
        CreateSubSection (str, style);
        }

    public static void CreateSubSection(string str, GUIStyle skin = null)
        {
        EditorGUILayout.LabelField (str, style);
        }
    }

    
