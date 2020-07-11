using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace WorldBuilder
    {
    [EditorTool ("World Builder - Rotation Snap ")]
    public class SnapRotationTool : EditorTool
        {
        private Texture2D icon;
        private GUIContent content;

        private void OnEnable()
            {
            content = new GUIContent ()
                {
                image = icon,
                text = "Rotation Snap Tool",
                tooltip = "Rotation Snap Tool"
                };
            }

        public override GUIContent toolbarIcon
            {
            get { return content; }
            }

        // This is called for each window that your tool is active in. Put the functionality of your tool here.
        public override void OnToolGUI(EditorWindow window)
            {
            Quaternion before = Tools.handleRotation;
            Vector3 handlePosition = Tools.handlePosition;

            EditorGUI.BeginChangeCheck ();

            Quaternion rotation = Handles.RotationHandle (before, Tools.handlePosition);
            float num = Mathf.Round (360 / WorldBuilderPrefs.snapAngle);

            rotation = Quaternion.Euler (
                Snapping.Snap (rotation.eulerAngles, WorldBuilderPrefs.snapAngle * Vector3.one));

            //for (int i = 0; i < num; i++)
            //    {
            //    float r = HandleUtility.GetHandleSize (handlePosition);

            //    float x = r * Mathf.Cos (i * WorldBuilderPrefs.snapAngle);
            //    float y = r * Mathf.Sin (i * WorldBuilderPrefs.snapAngle);

            //    //Handles.DrawLine (handlePosition, handlePosition + new Vector3 (x, y, 0));
            //    //Handles.DrawLine (handlePosition, handlePosition + new Vector3 (0, x, y));
            //    //Handles.DrawLine (handlePosition, handlePosition + new Vector3 (y, 0, x));
            //    }

            if (EditorGUI.EndChangeCheck ())
                {
                Undo.RecordObjects (Selection.transforms, "Rotate Position");

                foreach (var transform in Selection.transforms)
                    {
                    //Quaternion delta = rotation * Quaternion.Inverse (transform.rotation);
                    //transform.Rotate (delta.eulerAngles);

                    //Tools.handleRotation = Quaternion.identity;

                    //Taken from BuiltInTools.cs
                    //Don't generally think this makes any difference to the above for the most part, but I guess pivot checks are important

                    // Rotate around handlePosition (Global or Local axis).
                    Quaternion delta = Quaternion.Inverse (before) * rotation;
                    delta.ToAngleAxis (out float angle, out Vector3 axis);

                    Undo.RecordObjects (Selection.transforms, "Rotate");

                    //Rotate
                    foreach (Transform t in Selection.transforms)
                        {
                        // Rotate around handlePosition (Global or Local axis).
                        if (Tools.pivotMode == PivotMode.Center)
                            t.RotateAround (handlePosition, before * axis, angle);
                        else
                            t.Rotate (before * axis, angle, Space.World);
                        }
                    Tools.handleRotation = rotation;
                    }
                }
            }
        }
    }
    
