using UnityEngine;

namespace WorldBuilder
    { 
    public static class WorldBuilderConstants
        {
        // --- Colors ---
        public readonly static Color defaultBackColor = new Color (194 / 255f, 150f / 194, 150f / 194, 1);

        // --- Positioning ---
        public readonly static Rect open = new Rect (0, 0, 200, 300);
        public readonly static Rect closed = new Rect (0, 0, 200, 18);
        }
    }
