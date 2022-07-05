using UnityEditor;
using UnityEngine;

namespace naxokit.Styles
{
    public class DrawLine
    {
        public static void DrawHorizontalLine(int _height)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, _height);
            rect.height = _height;
            EditorGUI.DrawRect(rect, Color.white);

        }

    }
}
