using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HierarchyDecorator
    {

    [InitializeOnLoad]
    internal static class HierarchyDecorator
        {
        //Window cache
        private static EditorWindow _window;

        //Prefix Collections for Components        
        private static HierarchyDecoratorSettings settings => HierarchyDecoratorSettings.GetOrCreateSettings ();
        private static List<PrefixData> prefixes = settings.prefixList;

        //Collections
        private static Dictionary<Type, string> componentTypes = new Dictionary<Type, string> ()
            {
                { typeof(Animation),        "A"     },
                { typeof(Animator),         "An"    },

                { typeof(AudioListener),    "Ad"    },
                { typeof(AudioSource),      "As"    },

                //-- Coll --
                { typeof(BoxCollider),      "Cb"  },
                { typeof(SphereCollider),   "Cs"  },
                { typeof(CapsuleCollider),  "Cc"  },
                { typeof(MeshCollider),     "Cm"  },

                //-- Camera --
                { typeof(Camera),           "Cam"   },

                 //-- Lighting --
                { typeof(Light),            "L"     },

                //-- Nav --
                { typeof(NavMeshAgent),     "Nm"     },
                { typeof(NavMeshBuilder),   "Nb"     },

                //-- Mesh --
                { typeof(MeshRenderer),     "Mr"     },

                { typeof(ParticleSystem),   "Ps"    },
                { typeof(Projector),        "Pr"    },

                { typeof(Rigidbody),        "Rb"    },

                { typeof(RectTransform),    "Rt"    },
                { typeof(Image),            "Im"    },
                { typeof(RawImage),         "Rm"    },

                { typeof(Terrain),          "T"     },
            };

        private static List<Type> returnedComponents = new List<Type> ();

        //Defaults cached
        private static Color selectionColor = new Color (58f / 255f, 178f / 255f, 178f / 255f, 1);
        private static Color defaultHoverColor = new Color (150f / 255f, 150f / 255f, 150f / 255f, 1);

        private static int currentID;
        private static Type type;

        static string[] layerMasks = new string[32];
        static int maxNum = 0;

        static HierarchyDecorator()
            {
            List<string> s = new List<string>();
            for (int i = 0; i < 32; i++)
                {
                string ss = LayerMask.LayerToName (i);
                layerMasks[i] = ss;
                }

            EditorApplication.hierarchyWindowItemOnGUI -= HandleObject;
            EditorApplication.hierarchyWindowItemOnGUI += HandleObject;

            //var type = typeof (EditorWindow).Assembly.GetType ("UnityEditor.SceneHierarchyWindow");
            //var methodInfo = type.GetMethod ("SetExpandedRecursive");
            //type = typeof (EditorWindow).Assembly.GetType ("UnityEditor.SceneHierarchyWindow");
            //Debug.Log (type);

            //EditorApplication.ExecuteMenuItem ("Window/Hierarchy");
            //var window = EditorWindow.focusedWindow;

            //methodInfo.Invoke (window, new object[] { go.GetInstanceID (), expand });
            }

        private static void HandleObject(int instanceID, Rect selectionRect)
            {
            GameObject obj = EditorUtility.InstanceIDToObject (instanceID) as GameObject;
            currentID = instanceID;

            if (obj != null)
                {
                DrawElementStyle (obj, selectionRect);
                DrawComponentData (obj, selectionRect);

                Rect largeRect = selectionRect;
                largeRect.x = 0;
                largeRect.width += selectionRect.x;

                if (largeRect.Contains(Event.current.mousePosition))
                    DrawToggles (obj, selectionRect);
                }
            }
        private static int currentLayer;

        #region Draw

        /// <summary>
        /// Set the design of the element in the hierarchy
        /// </summary>
        /// <param name="obj">Current object element</param>
        /// <param name="selectionRect">Rect for the element</param>
        private static void DrawElementStyle(GameObject obj, Rect selectionRect)
            {
            currentLayer = obj.layer;

           

            //Generally an condition to wrap drawing correctly
            if (!prefixes.Any(p => obj.name.StartsWith(p.prefix)))
                {
                DrawLayerMask (obj, selectionRect);
                }
            else
                {
                //Check if prefix exists within the name of the GO
                foreach (PrefixData p in prefixes)
                    {
                    if (obj.name.StartsWith (p.prefix))
                        {
                        selectionRect.x += 28f;
                        selectionRect.width -= 28f*1.73f;

                        //If the current object is selected ignore formatting
                        //if (Selection.gameObjects.Contains (obj))
                        //    {
                        //    EditorGUI.DrawRect (selectionRect, selectionColor);
                        //    ApplyElementStyle (selectionRect, obj.name, p, defaultHoverColor);
                        //    }
                        //else
                        //    {
                        //    ApplyElementStyle (selectionRect, obj.name, p);
                        //    }

                        ApplyElementStyle (selectionRect, obj.name, p);

                        //Return for only one prefix
                        return;
                        }
                    }
                }
            }

        /// <summary>
        /// Override the design of the element in the hiearchy
        /// </summary>
        /// <param name="selectionRect">Rect for the element</param>
        /// <param name="name">Original element name</param>
        /// <param name="backgroundColor">Background colour to apply</param>
        private static void ApplyElementStyle(Rect selectionRect, string name, PrefixData prefix)
            {
            GUIStyle style = settings.GetGUIStyle (prefix.style);

            selectionRect = CalculateElementSize (selectionRect);

            RemovePrefix (ref name, prefix.prefix);
            //RemoveAllPrefixesFromString (ref name);

            //Draw background and text
            //EditorGUI.DrawRect (selectionRect, prefix.color);

            EditorGUI.DrawRect (selectionRect, prefix.color);

            EditorGUI.LabelField (selectionRect, name.ToUpper (), style);

            DrawElementLinage (selectionRect, prefix);
            }

        /// <summary>
        /// Override the design of the element in the hiearchy
        /// </summary>
        /// <param name="selectionRect">Rect for the element</param>
        /// <param name="name">Original element name</param>
        /// <param name="backgroundColor">Background colour to apply</param>
        private static void ApplyElementStyle(Rect selectionRect, string name, PrefixData prefix, Color backgroundColour)
            {
            GUIStyle style = settings.GetGUIStyle (prefix.style);

            Rect elementRect = CalculateElementSize (selectionRect);

            RemovePrefix (ref name, prefix.prefix);
            //RemoveAllPrefixesFromString (ref name);

            //Draw background and text
            EditorGUI.DrawRect (elementRect, backgroundColour);
            EditorGUI.LabelField (elementRect, name.ToUpper (), style);

            DrawElementLinage (elementRect, prefix);
            }

        /// <summary>
        /// Draw Lines above and below the element depending on the option selected
        /// </summary>
        /// <param name="selectionRect">Element draw rect</param>
        /// <param name="options">Selected draw option</param>
        private static void DrawElementLinage(Rect selectionRect, PrefixData data)
            {
            switch (data.lineOptions)
                {
                case PrefixData.LineOptions.NONE:
                    break;

                case PrefixData.LineOptions.TOP:
                    GUIExtension.CreateLineSpacer (selectionRect, data.lineColor, 1f);
                    break;

                case PrefixData.LineOptions.BOTTOM:
                    selectionRect.y += EditorGUIUtility.singleLineHeight * 0.85f;
                    GUIExtension.CreateLineSpacer (selectionRect, data.lineColor, 1f);
                    break;

                case PrefixData.LineOptions.BOTH:
                    GUIExtension.CreateLineSpacer (selectionRect, data.lineColor, 1f);
                    selectionRect.y += EditorGUIUtility.singleLineHeight * 0.85f;
                    GUIExtension.CreateLineSpacer (selectionRect, data.lineColor, 1f);
                    break;

                default:
                    break;
                }
            }

        private static void DrawLayerMask(GameObject obj, Rect selectionRect)
            {
            maxNum = 3;

            selectionRect.y += 2;
            EditorGUI.LabelField (GetRightLayerMaskRect(selectionRect, maxNum), LayerMask.LayerToName (obj.layer), EditorStyles.centeredGreyMiniLabel);
            }

        /// <summary>
        /// Draw the common components found on the GameObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="selectionRect"></param>
        private static void DrawComponentData(GameObject obj, Rect selectionRect)
            {

            //Get end of area
            //Rect componentRect = GetWidthRect (selectionRect);
            //componentRect.x -= 16;

            //Debug.Log (selectionRect);
            //Clear components for new object
            returnedComponents.Clear ();

            string s = "";
            Type type = null;

            Color tint = Color.white;
            if (!obj.activeSelf)
                tint = new Color (0.9f, 0.9f, 0.9f, 0.4f);

            maxNum += 1;
            //Iterate over all components
            foreach (Component component in obj.GetComponents<Component> ())
                {
                if (component == null)
                    continue;

                type = component.GetType ();

                //Check if the component type is allowed to be drawn
                if (!IsAllowedType (ref type))
                    continue;

                //Ignore same type duplicates as they've been handled
                if (returnedComponents.Contains (type))
                    continue;

                returnedComponents.Add (type);

                Rect r = GetRightRectWithOffset (selectionRect, maxNum);

                //GUILayout.Label ();
                if (EditorGUIUtility.ObjectContent (null, type).image != null)
                    {
                    GUI.DrawTexture (r, EditorGUIUtility.ObjectContent (null, type).image, ScaleMode.ScaleToFit, true, 0, tint, 0, 0);
                    maxNum++;
                    }

                }

          
            }

        /// <summary>
        /// Draw toggles for the GameObject's active state
        /// </summary>
        private static void DrawToggles(GameObject obj, Rect selectionRect)
            {
            selectionRect.x = 32;
            selectionRect.width = 16f;

            obj.SetActive (EditorGUI.Toggle (selectionRect, obj.activeSelf, EditorStyles.toggle));
            }

        #endregion

        #region Checks

        private static bool IsAllowedType(ref Type type)
            {
            if (componentTypes.ContainsKey (type))
                {
                return true;
                }

            if (type.BaseType == null)
                {
                return false;
                }

            type = type.BaseType;

            return IsAllowedType (ref type);
            }

        #endregion

        #region Rect Helpers

        /// <summary>
        /// Return a Rect equal to the size of the element, but begins at x equal to the width
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private static Rect GetWidthRect(Rect rect)
            {
            rect = CalculateElementSize (rect);
            rect.x = rect.width;

            return rect;
            }

        /// <summary>
        /// Full size of the element 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private static Rect CalculateElementSize(Rect rect)
            {
            rect.x -= 28f;
            rect.width += 44f;

            return rect;
            }

        private static Rect GetRightRectWithOffset(Rect rect, int offset)
            {
            var newRect = new Rect (rect);
            newRect.width = newRect.height;
            newRect.x = rect.x + rect.width - (rect.height * offset) - 16;

            return newRect;
            }

        private static Rect GetRightLayerMaskRect(Rect rect, int offset)
            {
            rect = GetRightRectWithOffset (rect, offset);

            rect.x -= 4;
            rect.y -= 2;
            rect.width += 48;

            return rect;
            }

        #endregion

        #region Prefix Helpers

        private static string RemoveAllPrefixesFromString(ref string originalString)
            {
            foreach (PrefixData p in prefixes)
                {
                originalString = originalString.Trim (p.prefix.ToCharArray ()).Trim ();
                }

            return originalString;
            }

        private static void TogglePrefix(GameObject gameObject, PrefixData prefix)
            {
            string str = gameObject.name;

            //Clear all, or add the prefix
            if (str.StartsWith (prefix.prefix))
                {
                RemoveAllPrefixesFromString (ref str);
                }
            else
                {
                //Just in case some others exist
                RemoveAllPrefixesFromString (ref str);
                str = $"{prefix.prefix} {str}";
                }

            gameObject.name = str;
            }

        private static void RemovePrefix(ref string str, string prefix)
            {
            str = str.Remove (str.IndexOf (prefix), prefix.Length);
            }

        #endregion

        #region Menu Options

        [MenuItem ("GameObject/Designer/Toggle Header %h", priority = 100)]
        private static void CreateHeader()
            {
            TogglePrefix (Selection.gameObjects[0], prefixes[0]);
            }

        [MenuItem ("GameObject/Designer/Toggle Subheader %j", priority = 100)]
        private static void CreateSubheader()
            {
            TogglePrefix (Selection.gameObjects[0], prefixes[1]);
            }

  
        #endregion
        }
    }