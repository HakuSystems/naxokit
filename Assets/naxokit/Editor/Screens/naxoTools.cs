using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Styles;
using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class naxoTools : EditorWindow
    {
        public static void HandleToolsOpend()
        {
            DrawLine.DrawHorizontalLine();
            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                DrawLine.DrawHorizontalLine(1, Color.magenta);
                EditorGUILayout.LabelField("AudioSourceVolumeControl", EditorStyles.boldLabel);
                if (GUILayout.Button("AudioSourceVolumeControl"))
                {
                    AudioSourceVolumeControl.ShowWindow();
                }
                DrawLine.DrawHorizontalLine(1, Color.magenta);
                DrawLine.DrawHorizontalLine(1, Color.magenta);
                EditorGUILayout.LabelField("MassImporter", EditorStyles.boldLabel);
                if (GUILayout.Button("MassImporter"))
                {
                    MassImporter.ShowWindow();
                }
                DrawLine.DrawHorizontalLine(1, Color.magenta);
            }
            EditorGUILayout.EndVertical();

            DrawLine.DrawHorizontalLine();

        }
    }
}



