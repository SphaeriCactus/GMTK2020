using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditorInternal;
using System;

public class BuildToolsData : ScriptableObject
    {
    private bool addVersioning = false;

    private static BuildPlayerOptions options = new BuildPlayerOptions ()
        {
        target = BuildTarget.StandaloneWindows,
        options = BuildOptions.Development,
        };

    }

//TODO: Create SO for build tools
public class BuildToolsWindow : EditorWindow
    {
    //Build settings
    private string prefBuildFolder = "";
    private string prefBuildName = "";
    private bool addVersioning = false;

    private static BuildPlayerOptions options = new BuildPlayerOptions ()
        {
        target = BuildTarget.StandaloneWindows,
        options = BuildOptions.Development,
        };

    private static readonly string[] buttonLabel = { "PC", "Mac", "Both" };

    // --- GUIStyles ---
    private static GUIStyle largeBold;
    private static GUIStyle smallBold;
    private static GUIStyle smallLabel;
    private static GUIStyle titleStyle;

    // --- Scene Data ---
    private static ReorderableList list;
    private static Vector2 scrollPosition;

    // --- Properties ---
    private bool HasBuildKey => PlayerPrefs.HasKey ("BUILD_BuildFolder");

    private string defaultPath => Application.dataPath;
    private readonly string defaultName = "New Build";

    private void OnEnable()
        {
        this.name = "Build Tools";
        this.minSize = new Vector2 (310, this.minSize.y);

        if (list == null)
            {
            list = new ReorderableList (null, typeof (SceneAsset), true, true, true, true);

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                SceneAsset element = (SceneAsset)list.list[index];

                EditorGUI.PrefixLabel (rect, new GUIContent (index.ToString ()));
                list.list[index] = EditorGUI.ObjectField (
                    new Rect (rect.x + 18, rect.y, rect.width - 18, EditorGUIUtility.singleLineHeight), 
                    element, typeof (SceneAsset), false);
                };

            list.drawHeaderCallback = (Rect rect) =>
                {
                rect.width *= 0.5f;
                EditorGUI.LabelField (new Rect(rect.x, rect.y, rect.width/2, rect.height), "Build Scenes");

                rect.x += rect.width;
                if (GUI.Button(rect, "Save Scenes to Build"))
                    SaveToBuild ();
                };

            list.onAddCallback = (ReorderableList list) =>
                {
                list.list.Add (null);
                };
            }

        CheckValidation ();
        GetBuildScenes ();

        EditorBuildSettings.sceneListChanged -= GetBuildScenes;
        EditorBuildSettings.sceneListChanged += GetBuildScenes;
        }

    private void OnGUI()
        {
        largeBold = new GUIStyle (EditorStyles.label)
            {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16
            };

        smallLabel = new GUIStyle (EditorStyles.label)
            {
            alignment = TextAnchor.MiddleCenter,
            };

        smallBold = new GUIStyle (smallLabel)
            {
            fontStyle = FontStyle.Bold
            };


        if (!HasBuildKey)
            SetupInitial ();
        else
            DrawBuildSettings ();
        }

    #region Initalization

    private void CheckValidation()
        {
        if (HasBuildKey)
            {
            prefBuildFolder = PlayerPrefs.GetString ("BUILD_BuildFolder");
            prefBuildName = PlayerPrefs.GetString ("BUILD_BuildName");
            }
        else
            {
            prefBuildFolder = defaultPath;
            prefBuildName = defaultName;

            //PlayerPrefs.SetString ("TRIB_BuildFolder", prefBuildFolder);
            PlayerPrefs.SetString ("BUILD_BuildName", prefBuildName);
            }
        }

    private void SetupInitial()
        {
        Rect r = EditorGUILayout.GetControlRect ();
        r.height = 150;
        r.width = position.width + r.x;
        r.x = 0;
        r.y = position.height / 2 - r.height / 4 - 24f;

        EditorGUI.DrawRect (r, Color.gray);

        GUILayout.FlexibleSpace ();
            InitFolderSelection ();
        GUILayout.FlexibleSpace ();
        }

    private void InitFolderSelection()
        {
        //Base info
        DirectorySettings ();
        GUIExtension.CreateLineSpacer ();
        GUILayout.Label ("Please choose a default folder for creating builds", smallLabel);
        }

    #endregion

    #region Draw
    private void DrawBuildSettings()
        {
        DirectorySettings ();
        BuildParameters ();
        SceneSelector ();

        CreateBuild ();
        }

    private void DirectorySettings()
        {
        GUILayout.Space (16);
        GUILayout.Label ("Build Platform", largeBold);

            GUILayout.BeginHorizontal ();

            prefBuildFolder = EditorGUILayout.DelayedTextField ("Build Folder", prefBuildFolder);

            if (GUILayout.Button ("Browse...", GUILayout.MaxWidth (80f)))
                prefBuildFolder = GetFolder ();

        GUILayout.EndHorizontal ();

        //Draw build path
        GUILayout.BeginHorizontal ();
            prefBuildName = EditorGUILayout.DelayedTextField ("Build File Name", prefBuildName);

            EditorGUI.BeginDisabledGroup (true);
                EditorGUILayout.TextField (".exe", GUILayout.MaxWidth(32f));
            EditorGUI.EndDisabledGroup ();
        GUILayout.EndHorizontal ();

        addVersioning = GUILayout.Toggle (addVersioning, "Add Version to Name");

        if (GUILayout.Button("Save Settings"))
            {
            PlayerPrefs.SetString ("BUILD_BuildName", prefBuildName);
            PlayerPrefs.SetInt ("BUILD_BuildAddVersion", addVersioning ? 1 : 0);

            if (!Directory.Exists (prefBuildFolder))
                prefBuildFolder = defaultPath;
            else
                PlayerPrefs.SetString ("BUILD_BuildFolder", prefBuildFolder);
            }

        }

    private void BuildParameters()
        {
        GUILayout.Space (16);
        GUILayout.Label ("Build Player Options", largeBold);

        options.target = (BuildTarget) EditorGUILayout.EnumPopup ("Target Platform", options.target);
        options.options = (BuildOptions) EditorGUILayout.EnumPopup ("Build Options", options.options);
        }

    //Scene
    private void SceneSelector()
        {
        GUILayout.Space (16);
        GUILayout.Label ("Scene Selector", largeBold);

        GUILayout.BeginHorizontal ();
        if (GUILayout.Button ("Load all project scenes"))
            GetAllScenes ();

        if (GUILayout.Button ("Load Build Scenes"))
            GetBuildScenes ();
        GUILayout.EndHorizontal ();


        GUILayout.BeginHorizontal ();

        if (GUILayout.Button ("Remove null scenes"))
            {
            var scenes = new List<SceneAsset> ();

            for (int i = 0; i < list.list.Count; i++)
                {
                if (list.list[i] != null)
                    scenes.Add ((SceneAsset)list.list[i]);
                }

            list.list = scenes;
            }

        if (GUILayout.Button ("Clear all scenes"))
            list.list.Clear ();

        GUILayout.EndHorizontal ();

        scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);
            {
            list.DoLayoutList ();
            }
        EditorGUILayout.EndScrollView ();
        }

    private void CreateBuild()
        {

        if (GUILayout.Button ("Create Build"))
            {
            var scenes = GetScenePaths ();

            Debug.Log ("build");

            try
                {
                string build = prefBuildFolder + prefBuildName;

                if (addVersioning)
                    build += Application.version;

                options.scenes = scenes;
                options.locationPathName = prefBuildFolder + "/" + prefBuildName  + ".exe";

                BuildPipeline.BuildPlayer (options);
                }
            catch (Exception e)
                {
                Debug.Log (e);
                }
            }

        GUILayout.Space (16f);
        }
    #endregion

    #region Menu Items

    [MenuItem ("Wooshii/Build Tools/Build Window")]
    public static void ShowWindow()
        {
        GetWindow (typeof (BuildToolsWindow));
        }

    [MenuItem ("Wooshii/Build Tools/Default Window")]
    public static void ShowDefaultWindow()
        {
        BuildPlayerWindow.ShowBuildPlayerWindow ();
        }

    [MenuItem ("Wooshii/Build Tools/Clear Build Data")]
    public static void ClearPrefData()
        {
        PlayerPrefs.DeleteKey ("BUILD_BuildFolder");
        PlayerPrefs.DeleteKey ("BUILD_BuildName");
        }

    #endregion

    #region Helpers

    private string GetFolder()
        {
        return EditorUtility.SaveFolderPanel ("Build Folder Path", defaultPath, defaultPath);
        }

    private void GetBuildScenes()
        {
        List<SceneAsset> scenes = new List<SceneAsset>();

        foreach (var scene in EditorBuildSettings.scenes)
            {
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset> (scene.path);

            if (asset != null)
                {
                if (!scenes.Contains (asset))
                    scenes.Add (asset);
                }
            }

        list.list = scenes;
        }

    private void GetAllScenes()
        {
        List<SceneAsset> scenes = new List<SceneAsset>();
        string[] allScenes = AssetDatabase.FindAssets ("t:SceneAsset");

        foreach (var scene in allScenes)
            {
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset> (AssetDatabase.GUIDToAssetPath(scene));

            if (asset != null)
                {
                if (!scenes.Contains (asset))
                    scenes.Add (asset);
                }
            }

        list.list = scenes;
        }

    private void SaveToBuild()
        {
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene> ();
        foreach (var scene in list.list)
            {
            if (scene == null)
                {
                Debug.LogError ("Cannot build with null scene reference!");
                return;
                }

            SceneAsset sceneAsset = (SceneAsset)scene;

            string scenePath = AssetDatabase.GetAssetPath (sceneAsset);

            if (!string.IsNullOrEmpty (scenePath))
                editorBuildSettingsScenes.Add (new EditorBuildSettingsScene (scenePath, true));
            }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray ();
        }

    private string[] GetScenePaths()
        {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
            scenes[i] = EditorBuildSettings.scenes[i].path;

        return scenes;
        }

    #endregion
    }
