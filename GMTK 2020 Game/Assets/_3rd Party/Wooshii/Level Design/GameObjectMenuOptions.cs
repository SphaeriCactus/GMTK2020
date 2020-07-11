using UnityEditor;
using UnityEngine;

public class GameObjectMenuOptions
    {
    private static bool done;

    #region Grouping
    [MenuItem ("GameObject/Group %g", true, 0)]
    public static bool ValidateGroup()
        {
        return Selection.transforms.Length > 0;
        }

    [MenuItem ("GameObject/Group %g", false, 0)]
    public static void Group()
        {
        // prevent multiple execution when invoked via context menu


        // get the last top-level transform in selection
        Transform top = Selection.transforms[0];

        //Upper-most transform in heirarchy
        foreach (Transform transform in Selection.transforms)
            {
            if (transform == top)
                continue;

            //Obviously will be not a child of another or at least isn't a child of the current upper most
            if (transform.parent == null || !transform.IsChildOf (top.parent))
                {
                if (transform.parent != top.parent || transform.GetSiblingIndex () > top.GetSiblingIndex ())
                    top = transform;
                }
            }

        // Create the group root gameobject
        GameObject groupRoot = new GameObject ("New Group");
        Undo.RegisterCreatedObjectUndo (groupRoot, "Undo Creation of Group");

        Undo.SetTransformParent (groupRoot.transform, top.parent, "Undo Creation of Group");

        // Calculate the group root new position (average point)
        Vector3 averagePoint = Vector3.zero;

        foreach (Transform t in Selection.transforms)
            averagePoint += t.position;

        averagePoint /= Selection.transforms.Length;
        groupRoot.transform.position = averagePoint;

        // re-parent transforms in selection
        foreach (Transform t in Selection.transforms)
            Undo.SetTransformParent (t, groupRoot.transform, "Undo Creation of Group");

        //groupRoot.transform.SetSiblingIndex (top.GetSiblingIndex ());
        Selection.activeGameObject = groupRoot;

        done = true;
        }
    #endregion
    }
