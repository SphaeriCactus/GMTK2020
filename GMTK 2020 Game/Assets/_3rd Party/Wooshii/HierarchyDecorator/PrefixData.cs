using UnityEngine;

namespace HierarchyDecorator
    {
    [System.Serializable]
    public class PrefixData
        {
        public enum LineOptions { NONE, TOP, BOTTOM, BOTH }
        //Prefix Data
        public string prefix;

        //Draw Options
        public Color color;
        public LineOptions lineOptions;
        public Color lineColor = Color.black;

        //GUIStyle
        public string style;

        public PrefixData()
            {

            }

        public PrefixData(char prefixChar, Color color, string style = "Header")
            {
            this.prefix = new string (prefixChar, 3); //Saves time blah
            this.color = color;
            this.style = style;

            lineOptions = LineOptions.BOTTOM;
            }

        public PrefixData(string prefixString, Color color, string style = "Header")
            {
            this.prefix = prefixString; //Saves time blah
            this.color = color;
            this.style = style;

            lineOptions = LineOptions.BOTTOM;
            }
        }
    }
