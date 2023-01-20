using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class Premium : EditorWindow
    {
        public static void HandlePremiumOpend()
        {
            GUILayout.Label("Thanks for the Support on Patreon!", EditorStyles.largeLabel);
            GUILayout.Label("Currently we dont have anything for Premium..", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
            }
            EditorGUILayout.EndVertical();
            
        }
    }
}
