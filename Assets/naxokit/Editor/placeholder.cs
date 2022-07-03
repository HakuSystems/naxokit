using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using naxokit;
using naxokit.Styles;

namespace naxokit
{
    public class placeholder : EditorWindow
    {
        bool boolToggle = false;
        [MenuItem("naxokit/test")]
        static void Init()
        {
            placeholder window = (placeholder)EditorWindow.GetWindow(typeof(placeholder));
            window.Show();
        }
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                var nanoSdkHeader = Resources.Load("Special") as Texture2D;
                boolToggle = FoldoutTexture.MakeTextureFoldout(nanoSdkHeader, boolToggle, 30f, 0, 0, 12f, 5f);
                if (boolToggle)
                {
                    GUILayout.Button("Test"); GUILayout.Button("Test"); GUILayout.Button("Test"); GUILayout.Button("Test"); GUILayout.Button("Test"); GUILayout.Button("Test"); GUILayout.Button("Test"); GUILayout.Button("Test");
                }
            }
            EditorGUILayout.EndVertical();

        }
    }

}