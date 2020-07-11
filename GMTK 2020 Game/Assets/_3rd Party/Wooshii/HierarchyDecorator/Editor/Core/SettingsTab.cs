using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HierarchyDecorator
    {
    internal class SettingsTab : SettingsProvider
        {
        #region Settings Tab

        public int tabSelected;
        public string[] pages = new [] { "Prefix Settings", "Icon Collection", "Modules" };

        #endregion

        #region Prefix Pages
        private Action onPageChange;

        private int maxLines = 3;
        private int displayPerLine = 3;
        private int prefixSelected = 0;

        private int currentPage = 1;

        private int MaxPages
            {
            get
                {
                if (settings == null || settings.prefixList == null)
                    return 0;
                else
                    return Mathf.CeilToInt((float)settings.prefixList.Count / (maxLines * displayPerLine));
                }
            }

        private int PrefixPageIndex => (maxLines * displayPerLine) * (currentPage - 1);
        private int PrefixElementsPerPage
            {
            get
                {
                if (settings == null || settings.prefixList == null || settings.prefixList.Count == 0)
                    return 1;
                else
                    //TOTAL AMOUNT = 18
                    //9 - (3 * 3) * (0) = 9
                    //12 - 9 * 1 = 3;
                    return Mathf.Min (settings.prefixList.Count, maxLines * displayPerLine, settings.prefixList.Count - ((maxLines * displayPerLine) * (currentPage - 1)) );
                }
            }

        #endregion

        #region Prefix Data

        private PrefixData currentPrefix;
        private SerializedProperty style;

        private PrefixData[] pageArray;
        private string[] prefixStrings;

        #endregion

        #region Settings Ref

        protected HierarchyDecoratorSettings settings;
        protected SerializedObject serialObject;

        private bool resetSettingsPrompt = false;

        #endregion

        //Constructor
        public SettingsTab(string path, SettingsScope scope = SettingsScope.User) : base (path, scope) { }

        //Activate and Deactivate for saving/loading
        public override void OnDeactivate()
            {
            base.OnDeactivate ();

            }
  
        // On Load of Window
        public override void OnActivate(System.String searchContext, VisualElement rootElement)
            {
            base.OnActivate (searchContext, rootElement);

            settings = HierarchyDecoratorSettings.GetOrCreateSettings ();

            if (settings)
                {
                serialObject = HierarchyDecoratorSettings.GetSerializedSettings ();
                //Debug.Log ($"Max Pages: {MaxPages} | Current Page: {currentPage} | " +
                //$"Current Amount: {settings.prefixList.Count} | Current Page Index: { PrefixPageIndex }");

                pageArray = settings.prefixList.GetRange (PrefixPageIndex, PrefixElementsPerPage).ToArray ();
                prefixStrings = pageArray.Select (s => s.prefix).ToArray ();
                }

            onPageChange += HandlePrefixSettings;
            }

        // GUI
        public override void OnTitleBarGUI()
            {
            base.OnTitleBarGUI ();
            tabSelected = GUILayout.SelectionGrid (tabSelected, pages, pages.Length, GUILayout.Height (50f));
            }

        public override void OnGUI(string searchContext)
            {
            if (settings.prefixList == null || settings.prefixList.Count == 0)
                {
                EditorGUILayout.Space (2f);
                EditorGUILayoutExt.AddSpacer ();

                PrefixListButtons ();
                return;
                }

            EditorGUILayout.LabelField (
                $"Max Pages: {MaxPages} | Current Page: {currentPage} | " +
                $"Current Amount: {settings.prefixList.Count} | Current Page Index: { PrefixPageIndex }");
            
            EditorGUILayout.Space (2f);
            EditorGUILayoutExt.AddSpacer ();

            switch (tabSelected)
                {
                case 0:
                    HandlePrefixSettings ();
                    break;

                case 1:

                    EditorGUILayoutExt.AddSpacer ();
                    break;

                case 2:

                    EditorGUILayoutExt.AddSpacer ();
                    break;
                }
            }

        public override void OnFooterBarGUI()
            {
            switch (tabSelected)
                {
                case 0:
                    if (resetSettingsPrompt)
                        {
                        EditorGUILayout.BeginVertical ();
                        EditorGUILayout.LabelField ("Are you sure you would like to reset all prefixes and styles?");

                        EditorGUILayout.BeginHorizontal ();
                        if (GUILayout.Button ("Yes"))
                            {
                            settings.SetDefaults ();
                            resetSettingsPrompt = false;
                            }

                        if (GUILayout.Button ("No"))
                            {
                            resetSettingsPrompt = false;
                            }

                        EditorGUILayout.EndHorizontal ();
                        EditorGUILayout.EndVertical ();
                        }
                    break;
                }

            base.OnFooterBarGUI ();
            }

        #region Prefix Settings
        private void HandlePrefixSettings()
            {
            EditorGUI.BeginChangeCheck ();

            EditorGUILayout.BeginVertical ();

            PrefixListButtons ();
            PrefixHeaderButtons ();


            //Draw the selection
            prefixSelected = (GUILayout.SelectionGrid (prefixSelected, prefixStrings, displayPerLine));

            DrawPrefixStyleSettings ();


            EditorGUILayout.EndVertical ();

            if (EditorGUI.EndChangeCheck ())
                {
                //Clamp
                currentPage = Mathf.Clamp (currentPage, 1, MaxPages);
                prefixSelected = Mathf.Clamp (prefixSelected, 0, settings.prefixList.Count);

                //Current prefix reference
                currentPrefix = pageArray[prefixSelected];

                //TODO: Stop being a dumbass and reference styles properly
                int index = settings.styles.IndexOf (settings.GetGUIStyle (currentPrefix.style));
                style = serialObject.FindProperty ("styles").GetArrayElementAtIndex (index);

                if (serialObject.ApplyModifiedProperties ())
                    EditorApplication.RepaintHierarchyWindow ();

                pageArray = settings.prefixList.GetRange (PrefixPageIndex, PrefixElementsPerPage).ToArray ();
                prefixStrings = pageArray.Select (s => s.prefix).ToArray ();
                }
            }

        private void PrefixHeaderButtons()
            {
            //Draw title and page buttons
            EditorGUILayout.BeginHorizontal();

                if (ConditionalButton ("<", MaxPages > 1 && currentPage != 1))
                    {
                    currentPage--;
                    onPageChange ();
                    }

            EditorGUILayout.LabelField ($"Selection ({currentPage}/{MaxPages})", settings.GetGUIStyle ("Header"));

            if (ConditionalButton (">", MaxPages > 1 && currentPage != MaxPages))
                    {
                    currentPage++;
                    onPageChange ();
                    }

                EditorGUILayout.EndHorizontal ();

            //Display Settings
            int displayMin = Mathf.Clamp (Mathf.Min (3, settings.prefixList.Count), 1, 3);
            int displayMax = Mathf.Min (settings.prefixList.Count, 8);

            }

        private void DrawPrefixStyleSettings()
            {
            EditorGUILayout.LabelField ("Settings", settings.GetGUIStyle ("Header"));

            if (currentPrefix == null)
                return;

            EditorGUILayout.BeginHorizontal ();

                //Prefix
                EditorGUILayout.BeginVertical ();

                    currentPrefix.prefix = EditorGUILayout.TextField ("Prefix", currentPrefix.prefix, EditorStyles.textField);
                    currentPrefix.lineOptions = (PrefixData.LineOptions)EditorGUILayout.EnumPopup ("Line Style", currentPrefix.lineOptions);

                EditorGUILayout.EndVertical ();

                //Colour //Line Option
                EditorGUILayout.BeginVertical ();
          
                    currentPrefix.color = EditorGUILayout.ColorField ("Background Colour", currentPrefix.color);

                    //Get Style
                    if (style != null)
                        EditorGUILayout.PropertyField (style, new GUIContent (style.displayName), true);
            EditorGUILayout.EndVertical ();

            EditorGUILayout.EndHorizontal ();

            

            }

        private void PrefixListButtons()
            {
            EditorGUILayout.BeginHorizontal ();

            if (GUILayout.Button ("Add New Prefix", EditorStyles.centeredGreyMiniLabel))
                {
                if (settings.prefixList == null)
                    settings.prefixList = new List<PrefixData> ();

                settings.prefixList.Add (new PrefixData ("PREFIX " + (settings.prefixList.Count + 1), Color.black));
                }

            if (GUILayout.Button ("Remove Selected", EditorStyles.centeredGreyMiniLabel))
                {
                Undo.RecordObject (settings, "Recovered deleted prefix");
                settings.prefixList.Remove (currentPrefix);

                if (PrefixElementsPerPage == 0)
                    tabSelected--;

                prefixSelected--;
                }

            if (GUILayout.Button ("Reset Settings", EditorStyles.centeredGreyMiniLabel))
                {
                //settings.SetDefaults ();
                resetSettingsPrompt = true;
                }

            EditorGUILayout.EndHorizontal ();
            }

        #endregion

        #region Helpers

        private bool ConditionalButton(string text, bool condition)
            {
            GUI.enabled = condition;
             bool button = GUILayout.Button (text);
            GUI.enabled = true;

            return button;
            }

        #endregion
        }
    }

