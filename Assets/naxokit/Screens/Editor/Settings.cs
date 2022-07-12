using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using naxokit.Styles;

namespace naxokit.Screens
{
    public class Settings : EditorWindow
    {
        public static void HandleSettingsOpend()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Comming Soon...", EditorStyles.centeredGreyMiniLabel);
        }
    }
}



