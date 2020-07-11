using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
    {
    // Create a new type of Settings Asset.
    internal class HierarchyDecoratorSettings : ScriptableObject
        {
      
        // --- Prefix ---
        public List<PrefixData> prefixList;

        private readonly List<PrefixData> importantPrefixes = new List<PrefixData>()
            {
            new PrefixData ('=', new Color (150f / 255f, 150f / 255f, 150f / 255f, 1), "Header"),
            new PrefixData ('-' ,new Color (178f / 255f, 178f / 255f, 178f / 255f, 1), "Toolbar"),
            new PrefixData ('+' ,new Color (63f / 255f, 188f / 255f, 200f / 255f, 1)),
            };

        // --- Styles ---
        public List<GUIStyle> styles = new List<GUIStyle> ();

        /// <summary>
        /// Load the asset for settings, or create one if it doesn't already exist
        /// </summary>
        /// <returns>The loaded settings</returns>
        internal static HierarchyDecoratorSettings GetOrCreateSettings()
            {
            //Find the asset 
            HierarchyDecoratorSettings settings = AssetDatabase.LoadAssetAtPath<HierarchyDecoratorSettings> (Constants.SETTINGS_ASSET_PATH);

            //Create the asset if it doesn't exist
            if (settings == null)
                {
                //Create and setup defaults
                settings = CreateInstance<HierarchyDecoratorSettings> ();
                settings.SetDefaults ();

                if (!Directory.Exists (Constants.SETTINGS_ASSET_FOLDER))
                    Directory.CreateDirectory (Constants.SETTINGS_ASSET_FOLDER);

                //Create and save
                AssetDatabase.CreateAsset (settings, Constants.SETTINGS_ASSET_PATH);
                AssetDatabase.SaveAssets ();

                //Call Debug
                Debug.LogAssertion ($"Hiearchy Decorator found no previous settings, creating one at {Constants.SETTINGS_ASSET_PATH}.");
                }
            return settings;
            }

        /// <summary>
        /// Convert into serialized object for handling GUI
        /// </summary>
        /// <returns>Serialized version of the settings</returns>
        internal static SerializedObject GetSerializedSettings()
            {
            return new SerializedObject (GetOrCreateSettings ());
            }

        /// <summary>
        /// Setup defaults for the new settings asset
        /// </summary>
        public void SetDefaults()
            {
            prefixList = new List<PrefixData> ();
            styles = new List<GUIStyle> ();

            prefixList = importantPrefixes;

            styles = new List<GUIStyle> ()
                {
                CreateGUIStyle ("Header",       EditorStyles.boldLabel),
                CreateGUIStyle ("Toolbar",      EditorStyles.toolbarButton),
                CreateGUIStyle ("Grid Centered",EditorStyles.centeredGreyMiniLabel),
                };

            styles[2].border = new RectOffset (15, 15, 15, 15);
            styles[2].stretchWidth = true;

            foreach (PrefixData data in prefixList)
                data.lineOptions = PrefixData.LineOptions.BOTTOM;
            }

        public GUIStyle GetGUIStyle(string name)
            {
            GUIStyle style = styles.SingleOrDefault (s => s.name == name);
            return style ?? EditorStyles.boldLabel;
            }

        private GUIStyle CreateGUIStyle(string name, GUIStyle styleBase = null)
            {
            //Generally optimistic settings
            return new GUIStyle (styleBase)
                {
                name = name,

                stretchHeight = false,
                stretchWidth = false,

                fontSize = 12,
                fontStyle = FontStyle.Bold,

                fixedHeight = 0,
                fixedWidth = 0,

                alignment = TextAnchor.MiddleCenter,
                };
            }
        }

    }
