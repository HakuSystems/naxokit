using UnityEditor;
using UnityEngine;

namespace naxokit.Styles
{
    public class DrawLine
    {
        public static void DrawHorizontalLine(int _height = 1, Color color = default)
        {
            if (color == default)
                color = Color.white;
            Rect r = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, _height, NaxoGUIStyleStyles.GUIStyleType.EyeDropperHorizontalLine.ToString());
            // Rect r = EditorGUILayout.GetControlRect(false, _height);
            r.height = _height;
            r.y += (_height - 1) / 2;
            EditorGUI.DrawRect(r, color);
        }

    }
}
