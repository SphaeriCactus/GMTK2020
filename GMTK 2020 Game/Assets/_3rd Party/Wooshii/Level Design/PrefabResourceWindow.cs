using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Reflection;

public class PrefabResourceWindow : EditorWindow
    {
    private enum DisplayType { LARGE_IMAGE, SMALL_IMAGE, IMAGE_TEXT_HORIZONTAL, IMAGE_TEXT_VERTICAL}
    private DisplayType display;
    private ImagePosition imagePosition = ImagePosition.ImageAbove;

    //Resource
    private Object selectedResource;
    private Editor previewEditor;

    private Object[] resources;
    private float assetLoadPercent;
    
    //Asset data
    private string resourceDir;
    private string defaultDir = "./Assets";
    private string fileType = ".prefab";

    private GUIContent[] content;

    //Editor
    private Vector2 resourceScroll;
    private int resourceIndex;

    [MenuItem ("Wooshii/Prefab Resources")]
    static void Intialize()
        {
        EditorWindow window = GetWindow (typeof (PrefabResourceWindow));
        window.ShowModalUtility ();
        }

    private void OnEnable()
        {
        resourceDir = defaultDir;
        }

    private void OnGUI()
        {
        string dir = resourceDir;

        EditorGUILayout.BeginVertical ();

        //Directory Set
        EditorGUILayout.LabelField ("Prefab Directory", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck ();

            dir = EditorGUILayout.DelayedTextField ("Base Prefab Folder", resourceDir, EditorStyles.toolbarSearchField);

            if (GUILayout.Button("Select Directory"))
                dir = EditorUtility.OpenFolderPanel ("Select Prefab Folder", defaultDir, defaultDir);

            if (!Directory.Exists (resourceDir))
                EditorGUILayout.HelpBox ("Cannot find directory " + resourceDir, MessageType.Error);

            if (EditorGUI.EndChangeCheck())        
                resourceDir = dir;

        EditorGUILayout.EndVertical ();

            if (GUILayout.Button ("Load Resources"))
                LoadResources (resourceDir);

//        GUIExtension.CreateLineSpacer ();   

        DrawResources ();

       

        //if (selectedResource != null)
        //    {
        //    if (previewEditor == null)
        //        previewEditor = Editor.CreateEditor (selectedResource);

        //    previewEditor.OnInteractivePreviewGUI (GUILayoutUtility.GetRect (256, 256), bgColor);
        //    }
        }


    private void DrawResources()
        {
        //Draw list of resources
        if (resources != null)
            {
            content = new GUIContent[resources.Length];

            for (int i = 0; i < resources.Length; i++)
                {
                content[i] = new GUIContent ()
                    {
                    text = resources[i].name,
                    image = AssetPreview.GetAssetPreview (resources[i])
                    };

                if (content[i].text.Length > 12)
                    content[i].text = content[i].text.Substring (0, 10) + "...";
                }
            }

        EditorGUILayout.BeginVertical (GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));

        //GUI.color = Color.Lerp (Color.red, Color.white, assetLoadPercent);
        //    EditorGUI.ProgressBar (EditorGUILayout.GetControlRect (), assetLoadPercent, "Loading...");
        //GUI.color = Color.white;

        EditorGUILayout.Space ();

        GUIStyle style = null;

        display = (DisplayType)EditorGUILayout.EnumPopup (display);
        imagePosition = (ImagePosition)EditorGUILayout.EnumPopup (imagePosition);
        int gridWidth = 2;

        switch (display)
            {
            case DisplayType.LARGE_IMAGE:
                gridWidth = Mathf.RoundToInt (position.width / 88);

                style = new GUIStyle (EditorStyles.whiteLargeLabel)
                    {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,

                    margin  = new RectOffset (1, 1, 1, 1),
                    padding = new RectOffset (0, 0, 0, 0),

                    fixedHeight = 96,
                    fixedWidth = 96,
                    };

                break;

            case DisplayType.SMALL_IMAGE:
                gridWidth = Mathf.FloorToInt (position.width / 54);

                style = new GUIStyle (EditorStyles.centeredGreyMiniLabel)
                    {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,

                    margin = new RectOffset (1, 1, 1, 1),
                    padding = new RectOffset (0, 0, 0, 0),

                    fixedHeight = 64,
                    fixedWidth = 64,
                    };
                break;

            case DisplayType.IMAGE_TEXT_HORIZONTAL:
                style = new GUIStyle (EditorStyles.toolbar)
                    {
                    stretchWidth = false,
                    stretchHeight = false
                    };
                break;

            case DisplayType.IMAGE_TEXT_VERTICAL:
                style = new GUIStyle (EditorStyles.objectField)
                    {
                    };
                break;
            default:
                break;
            }

        style.imagePosition = imagePosition;
        style.normal.textColor = Color.black;
        style.normal.background = GUI.skin.box.normal.background;

        resourceScroll = EditorGUILayout.BeginScrollView (resourceScroll);

        EditorGUI.BeginChangeCheck ();

        if (resources != null && content != null)
            {
            resourceIndex = GUILayout.SelectionGrid (resourceIndex, content, gridWidth, style);

            //EditorGUILayout.ObjectField (content[0], resources[0], typeof(Object), false);

            if (EditorGUI.EndChangeCheck ())
                {
                selectedResource = resources[resourceIndex];
                Selection.objects = new Object[] { selectedResource };
                //previewEditor = Editor.CreateEditor (selectedResource);
                }
            }

        EditorGUILayout.EndScrollView ();

        EditorGUILayout.EndVertical ();

      
        if (selectedResource != null)
            {
            Event evt = Event.current;

            switch (evt.type)
                {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                        {
                        DragAndDrop.AcceptDrag ();


                        }

                    break;
                }
                
            }
           
        }

    private void LoadResources(string directory)
        {
        var stopwatch = new System.Diagnostics.Stopwatch ();
        stopwatch.Start ();

        String[] files = Directory.GetFiles (directory, "*.prefab", SearchOption.AllDirectories);
        List<Object> objects = new List<Object> ();

        foreach (string file in files)
            {
            string assetPath = "Assets" + file.Replace (Application.dataPath, "").Replace ('\\', '/');
            objects.Add (AssetDatabase.LoadAssetAtPath (assetPath, typeof (Object)));
            }

        resources = objects.ToArray ();
        stopwatch.Stop ();

        var timerInfo = stopwatch.Elapsed;

        Debug.Log ("Loaded " + files.Length + " files in " + stopwatch.Elapsed.ToString ("mm\\:ss\\.ff") + " seconds.");
        }


    // oWo Special Stuffs o3o
    static Assembly assembly = Assembly.GetAssembly (typeof (Editor));
    static Type GetType(string typeName)
        {
        return assembly.GetType ($"UnityEditor.{typeName}");
        }
    static Type type = GetType ("Experimental.EditorResources+Constants");
    static bool isDarkTheme
        {
        set => isDarkThemeField?.SetValue (null, value);
        get => (bool)isDarkThemeField?.GetValue (null);
        }
    static FieldInfo isDarkThemeField = type?.GetField ("isDarkTheme");
    }

