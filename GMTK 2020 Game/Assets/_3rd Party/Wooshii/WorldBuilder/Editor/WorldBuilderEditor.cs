using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace WorldBuilder
    {
    public enum PrefType { STRING, BOOL, FLOAT, INT}

    [InitializeOnLoad]
    internal static class WorldBuilderEditor
        {
        // --- States ---
        private static bool showToolbox = true;

        //Rotation Snap Tool
        private static bool relativeSnap = false;
        private static float rotationSnap = 45;

        //Grid Instanciation Tool
        //private static float gridOffset = 0;

        // --- Settings ---
        private static readonly KeyCode duplicationKey = KeyCode.LeftAlt | KeyCode.RightAlt;
        private static readonly KeyCode rotationSnappingKey = KeyCode.LeftControl | KeyCode.RightControl;

        private static Event currentEvent;
        private static SceneView currentView;

        // --- Properties ---
        private static bool isPressingAlt = false;

        static WorldBuilderEditor()
            {
            WorldBuilderPrefs.Initialize ();
            isPressingAlt = false;

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            }

        private static void OnSceneGUI(SceneView view)
            {
            if (Tools.current != Tool.Custom)
                return;

            currentView = SceneView.currentDrawingSceneView;

            //Positioning setup
            Rect currentRect = (showToolbox) ? WorldBuilderConstants.open : WorldBuilderConstants.closed;
            Rect headerRect = currentRect;
            headerRect.x = 0;
            headerRect.y = 0;
            headerRect.height = 18;

            //Draw 
            Handles.BeginGUI ();
            GUILayout.BeginArea (currentRect);

            //Draw behind the foldout too
            EditorGUI.DrawRect (currentRect, WorldBuilderConstants.defaultBackColor);

            showToolbox = EditorGUI.BeginFoldoutHeaderGroup (headerRect, showToolbox, "World Builder Toolbox", EditorStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup ();

            //Underline header
            currentRect.y += headerRect.height;
            GUIExtension.CreateLineSpacer (currentRect, Color.gray, 1);


            //Show toolbox content
            if (showToolbox)
                {
                currentRect.y += 2;
                DrawToolboxContent (currentRect);
                }

            GUILayout.EndArea ();
            Handles.EndGUI ();

            Duplicate ();
            }

        private static void DrawToolboxContent(Rect currentRect)
            {
            GUILayout.BeginArea (currentRect);

            EditorGUI.BeginChangeCheck ();

            EditorGUILayout.LabelField ("Rotation Settings", EditorStyles.boldLabel);
                rotationSnap = EditorGUILayout.FloatField ("Rotation Snap Value", rotationSnap);
                relativeSnap = EditorGUILayout.Toggle ("Relative Rotation", relativeSnap);

            //EditorGUILayout.Space ();

            //EditorGUILayout.LabelField ("Grid Snap Settings", EditorStyles.boldLabel);
            //    gridOffset = EditorGUILayout.FloatField ("Grid Placement Offset", gridOffset);

            if (EditorGUI.EndChangeCheck ())
                {
                WorldBuilderPrefs.snapAngle = rotationSnap;
                WorldBuilderPrefs.relativeSnap = relativeSnap;
                //WorldBuilderPrefs.gridOffset = gridOffset;

                //Saving
                WorldBuilderPrefs.SetFloat (WorldBuilderPrefs.pref_snapSize, rotationSnap);
                WorldBuilderPrefs.SetFloat (WorldBuilderPrefs.pref_relativeSnap, rotationSnap);
                //WorldBuilderPrefs.SetFloat (WorldBuilderPrefs.pref_gridOffset, gridOffset);
                }

            GUILayout.EndArea ();
            }

        private static Vector3 prevHandlePos;

        // -- GameObject interactions --
        private static void Duplicate()
            {


            if (Selection.gameObjects.Length > 0 && Tools.current == Tool.Move)
                {
                if (!isPressingAlt)
                    {
                    isPressingAlt = Event.current.alt;

                    if (isPressingAlt)
                        {
                        List<GameObject> newObjects = new List<GameObject> ();
                        var selection = Selection.gameObjects;

                        for (int i = 0; i < selection.Length; i++)
                            {
                            GameObject original = selection[i];
                            int index = original.transform.GetSiblingIndex ();

                            newObjects.Add (Object.Instantiate (original));
                            }

                        Selection.objects = newObjects.ToArray ();
                        }
                    }
                else
                    {
                    if (!Event.current.alt)
                        {
                        isPressingAlt = false;
                        prevHandlePos = Tools.handlePosition;
                        }
                    }
                }

            }
        }

    internal static class WorldBuilderPrefs
        {
        // --- Rot Tool ---
        public static float snapAngle;
        public static string pref_snapSize = "WB_snapSize";

        public static bool relativeSnap;
        public static string pref_relativeSnap = "WB_relativeSnap";

        // --- Grid Tool ---
        //public static float gridOffset;
        //public static string pref_gridOffset = "WB_rotSnap";

        public static void Initialize()
            {
            //Rot
            snapAngle = EditorPrefs.HasKey (pref_snapSize) ? EditorPrefs.GetFloat(pref_snapSize) : 45;
            relativeSnap = EditorPrefs.HasKey (pref_relativeSnap) ? EditorPrefs.GetBool (pref_relativeSnap) : false;

            //Grid
            //gridOffset = EditorPrefs.HasKey (pref_gridOffset) ? EditorPrefs.GetFloat (pref_gridOffset) : 0; 
            }

        public static void SetFloat(string key, float value)
            {
            EditorPrefs.SetFloat (key, value);
            }

        public static void SetString(string key, string value)
            {
            EditorPrefs.SetString (key, value);
            }

        public static void SetBool(string key, bool value)
            {
            EditorPrefs.SetBool (key, value);
            }

        public static void SetInt(string key, int value)
            {
            EditorPrefs.SetInt (key, value);
            }
        }
    }
