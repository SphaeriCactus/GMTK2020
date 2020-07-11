using UnityEditor;
using UnityEngine;

namespace WooshiiAttributes
    {

    [CustomPropertyDrawer (typeof (ContainedClassAttribute))]
    public class ContainedClassDrawer : WooshiiPropertyDrawer
        {
        private GUIStyle style = new GUIStyle (EditorStyles.boldLabel);
        private float initalPadding = 2;

        private float expandedHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
            style.alignment = TextAnchor.UpperLeft;

            
            //Begin Draw
            EditorGUI.BeginProperty (position, label, property);

            //Rect
            Rect baseRect = position;
            baseRect.height = 19;

            //Title Rect
            Rect labelRect = EditorGUI.IndentedRect (baseRect);
            labelRect.x += 2;
            labelRect.y += initalPadding;

            //Child Rect
            Rect childRect = EditorGUI.IndentedRect (baseRect);
            childRect.height = SingleLine;
            childRect.width -= 5f;
            childRect.y += SingleLine*1.1f;

            //============ Draw ============
            DrawBackground (EditorGUI.IndentedRect(position), new Color (0, 0, 0, 0.15f));

            property.isExpanded = EditorGUI.Foldout (baseRect, property.isExpanded, "");
            GUI.Label (labelRect, property.displayName, style);


            if (property.isExpanded)
                {
                EditorGUI.indentLevel = 1;
                DrawChildProperties (property, ref childRect);

                baseRect.height += 200;
                }
            else
                {

                }

            EditorGUI.indentLevel = 0;
            EditorGUI.EndProperty ();
            }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
            if (property.isExpanded)
                return EditorGUI.GetPropertyHeight (property, label, true);
            else
                return EditorGUI.GetPropertyHeight (property);
            }

        private void DrawChildProperties(SerializedProperty property, ref Rect rect)
            {
            SerializedProperty itr = property.Copy ();
            SerializedProperty current = itr.Copy();

            bool iterateChildrenTemp = true;
            while (itr.NextVisible (iterateChildrenTemp))
                {
                iterateChildrenTemp = false;

                if (itr.hasVisibleChildren)
                    iterateChildrenTemp = itr.isExpanded;

                //Return if end
                if (SerializedProperty.EqualContents (itr, property.GetEndProperty ()))
                    break;

                EditorGUI.PropertyField (rect, itr);

                //Increase the height for the next property
                rect.y += EditorGUI.GetPropertyHeight (itr, null, false);
                rect.height = SingleLine;
                }

            expandedHeight = rect.y;
            }

        }
    }

