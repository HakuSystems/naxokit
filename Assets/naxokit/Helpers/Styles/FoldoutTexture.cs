using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace naxokit.Styles
{
    public class FoldoutTexture
    {
        public static bool MakeTextureFoldout(Texture2D texture2D, bool boolToggle, float floatHeight, float floatOffsetX, float floatOffsetY, float floatArrowOffsetY, float floatArrowOffsetX)
        {
            GUIStyle formatting = new GUIStyle("ShurikenModuleTitle");
            formatting.contentOffset = new Vector2(18.0f + floatOffsetX, -2.0f + floatOffsetY);
            formatting.fixedHeight = floatHeight;
            Rect rect = GUILayoutUtility.GetRect(5.0f, floatHeight, formatting);
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
