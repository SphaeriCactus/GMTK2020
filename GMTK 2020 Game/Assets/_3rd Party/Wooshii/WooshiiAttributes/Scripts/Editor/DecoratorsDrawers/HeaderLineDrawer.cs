using System;
using UnityEditor;
using UnityEngine;

namespace WooshiiAttributes
    {
    [CustomPropertyDrawer (typeof (HeaderLineAttribute))]
    public class HeaderLineDrawer : WooshiiDecoratorDrawer
        {
        private HeaderLineAttribute header => attribute as HeaderLineAttribute;
        private GUIStyle style = new GUIStyle (EditorStyles.boldLabel);

        public override void OnGUI(Rect rect)
            {
            //Draw label
            EditorGUI.LabelField (rect, header.text.ToUpper(), style);

            //Move to new line and set following line height
            rect.y += SingleLine + 2;
            rect.height = 1;

            //Draw spacer
            GUIExtension.CreateLineSpacer (EditorGUI.IndentedRect (rect), Color.grey, rect.height);
            }

        //How tall the GUI is for this decorator
        public override float GetHeight()
            {
            return SingleLine * 1.25f;
            }

        }
    }

