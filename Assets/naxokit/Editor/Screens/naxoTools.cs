using System;
using System.Collections;
using System.Net;
using System.Security.Permissions;
using JetBrains.Annotations;
using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Screens.Tools;
using naxokit.Styles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace naxokit.Screens
{
    public class naxoTools : EditorWindow
    {
        private static VideoPlayer _movie;
        public static void HandleToolsOpend()
        {
            EditorGUILayout.BeginHorizontal();
            {
                //todo: Dont spam IF STATEMENTS
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
                if (GUILayout.Button("PresetManager"))
                {
                    PresetManager.ShowWindow();
                }
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
                if (GUILayout.Button("EasySearch"))
                {
                    GetWindow<EasySearch>().Show();
                }
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
                if (GUILayout.Button("NaxoLoader"))
                {
                    GetWindow<NaxoLoader>().Show();
                }
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
                if (GUILayout.Button("BackupManager"))
                {
                    GetWindow<BackupManager>().Show();
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



