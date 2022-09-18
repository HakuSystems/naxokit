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
            EditorGUILayout.BeginHorizontal();
            {
                //https://www.youtube.com/watch?v=VGqsbhdmN6I
                if (GUILayout.Button("AudioSourceVolumeControl"))
                {
                    AudioSourceVolumeControl.ShowWindow();
                }
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
                if (GUILayout.Button("MassImporter"))
                {
                    MassImporter.ShowWindow();
                }
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}



