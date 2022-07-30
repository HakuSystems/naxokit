using UnityEngine;
using UnityEditor;
using naxokit.Styles;

namespace naxokit.Screens{
    public class PlayMode : EditorWindow {

        public static void HandlePlayModeOpend(){
            DrawLine.DrawHorizontalLine();
            EditorGUILayout.LabelField("Play Mode", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {

            }
            EditorGUILayout.EndVertical();
            DrawLine.DrawHorizontalLine();
        }
    }
}
