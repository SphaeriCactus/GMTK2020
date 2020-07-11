using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace WorldBuilder
    {
    [EditorTool ("World Builder - Object Tiling")]
    public class TileTool : EditorTool
        {
        //Base info
        private GUIContent content;

        private Bounds gridBounds;
        private Bounds selectionBounds;

        // State info
        private Vector3 currentPosition;

        // Selection info
        private bool HasSelection => Selection.transforms.Length > 0;
        private GameObject[] selection => Selection.gameObjects;

        private HashSet<Vector3Int> gridPositions;
        private List<GameObject> gameObjects;

        // --- Cached Arrow Data ---
        private Vector3[] cachedDirections = {
            new Vector3(1, 0, 0), new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0), new Vector3(0, -1, 0),
            new Vector3(0, 0, 1), new Vector3(0, 0, -1)};

        private Color[] directionColors = { Color.red, Color.green, Color.blue };

        private void OnEnable()
            {
            Initalize ();

            content = new GUIContent ()
                {
                image = null,
                text = "Object Tiler",
                tooltip = "Drag objects to tile instances of them over a grid",
                };

            Selection.selectionChanged -= Initalize;
            Selection.selectionChanged += Initalize;
            }

        public override GUIContent toolbarIcon => content;

        public override void OnToolGUI(EditorWindow window)
            {
            if (!HasSelection)
                return;

            //base.OnToolGUI (window);

            EditorGUI.BeginChangeCheck ();


            Vector3 arrowPos = Vector3.zero;
            Quaternion arrowRot = Quaternion.identity;
            Color arrowColor = Color.red;

            //currentPosition = Handles.PositionHandle (currentPosition, arrowRot);
            currentPosition = Handles.ScaleHandle (currentPosition, selectionBounds.min, arrowRot, HandleUtility.GetHandleSize (selectionBounds.min));

            //Calculate size of grid
            gridBounds.SetMinMax (
                selectionBounds.min,
                selectionBounds.extents + currentPosition
                );

            //Draw full sized and selection cubes
            Handles.color = Color.yellow;
                Handles.DrawWireCube (selectionBounds.center, selectionBounds.size);
 
            Handles.color = Color.white;
                Handles.DrawWireCube (gridBounds.center, gridBounds.size);

            if (EditorGUI.EndChangeCheck ())
                {
                Undo.RecordObjects (Selection.objects, "Added new objects");

                //Scale
                Vector3Int gridSize = Vector3Int.RoundToInt (gridBounds.size);

                if (selectionBounds.size.x != 0 && gridSize.x != 0)
                    gridSize.x /= Mathf.RoundToInt (selectionBounds.size.x);

                if (selectionBounds.size.y != 0 && gridSize.y != 0)
                    gridSize.y /= Mathf.RoundToInt (selectionBounds.size.y);

                if (selectionBounds.size.z != 0 && gridSize.z != 0)
                    gridSize.z /= Mathf.RoundToInt (selectionBounds.size.z);

                Vector3 position = Vector3.zero;
                for (int x = 0; x < gridSize.x; x++)
                    for (int y = 0; y < gridSize.y; y++)
                        for (int z = 0; z < gridSize.z; z++)
                            {
                            position = new Vector3 (x, y, z);

                            if (position == Vector3.zero || gridPositions.Contains(Vector3Int.RoundToInt(position)))
                                continue;

                            //Add before manipulation
                            gridPositions.Add (Vector3Int.RoundToInt (position));

                            position.Scale(selectionBounds.size);
                            position += selectionBounds.center;

                            GameObject original = Selection.gameObjects[0];
                            GameObject newObject = Instantiate (original, position, original.transform.rotation, null);
                            Undo.RegisterCreatedObjectUndo (newObject, "Added gameObject from TileTool");

                            gameObjects.Add(newObject);
                            }
                }
            }

        private void Initalize()
            {
            bool init = true;

            for (int i = 0; i < selection.Length; i++)
                {
                GameObject GO = selection[i];

                if (GO.TryGetComponent(out Renderer renderer))
                    {
                    Bounds bound = renderer.bounds;
                    Vector3 p = GO.transform.position;

                    if (init)
                        {
                        selectionBounds.SetMinMax (bound.min, bound.max);
                        }
                    else
                        {
                        selectionBounds.min = new Vector3 (
                            Mathf.Min (selectionBounds.min.x, bound.min.x),
                            Mathf.Min (selectionBounds.min.y, bound.min.y),
                            Mathf.Min (selectionBounds.min.z, bound.min.z)
                            );

                        selectionBounds.max = new Vector3 (
                            Mathf.Max (selectionBounds.max.x, bound.max.x),
                            Mathf.Max (selectionBounds.max.y, bound.max.y),
                            Mathf.Max (selectionBounds.max.z, bound.max.z)
                            );
                        }
                    }
                }

            gridBounds = selectionBounds;
            currentPosition = selectionBounds.center;

            gridPositions = new HashSet<Vector3Int> ();
            gameObjects = new List<GameObject> ();
            }
        }
    }
