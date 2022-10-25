using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using naxokit.Styles;
using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class Premium : EditorWindow
    {
        public static void HandlePremiumOpend()
        {
            GUILayout.Label("Thanks for the Support on Patreon!", EditorStyles.largeLabel);
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Box("You can now use the following features:", EditorStyles.boldLabel);
                GUILayout.Label("PLACEHOLDER", EditorStyles.helpBox);
                // GUILayout.Box("1. Customizable UI", EditorStyles.helpBox);
                // GUILayout.Box("2. Customizable Colors", EditorStyles.helpBox);
                // GUILayout.Box("3. Customizable Sounds", EditorStyles.helpBox);
                GUILayout.Label("PLACEHOLDER", EditorStyles.helpBox);
                
            }
            EditorGUILayout.EndVertical();
        }
    }
}
