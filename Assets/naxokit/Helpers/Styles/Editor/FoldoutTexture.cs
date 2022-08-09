using UnityEditor;
using UnityEngine;

namespace naxokit.Styles
{
    public class FoldoutTexture
    {
        //standard is : 30f, 0, 0, 12f, 5f
        public static bool MakeTextureFoldout(Texture2D texture2D, bool boolToggle, float floatHeight = 30f, float floatOffsetX = 0, float floatOffsetY = 0, float floatArrowOffsetY = 12f, float floatArrowOffsetX = 5f)
        {
            GUIStyle formatting = new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.ScriptText.ToString()); //ScriptText looks way better than ShurikenModuleTitle
            formatting.contentOffset = new Vector2(18.0f + floatOffsetX, -2.0f + floatOffsetY);
            formatting.fixedHeight = floatHeight;
            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, floatHeight, formatting);
            GUI.Box(rect, texture2D, formatting);
            return MakeTextureFoldoutState(boolToggle, rect, floatArrowOffsetY, floatArrowOffsetX);
        }
        private static bool MakeTextureFoldoutState(bool boolState, Rect rectSize, float floatOffsetY, float floatOffsetX)
        {
            Event currentEvent = Event.current;
            Rect arrowRect = new Rect(rectSize.x + floatOffsetX, rectSize.y + floatOffsetY, 0.0f, 0.0f);
            if (currentEvent.type == EventType.Repaint) EditorStyles.foldout.Draw(arrowRect, false, false, boolState, false);
            if (currentEvent.type == EventType.MouseDown && rectSize.Contains(currentEvent.mousePosition))
            {
                boolState = !boolState;
                currentEvent.Use();
            }
            return boolState;
        }
    }
}
