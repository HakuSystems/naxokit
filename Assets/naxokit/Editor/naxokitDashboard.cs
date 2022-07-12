using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using naxokit;
using naxokit.Styles;
using naxokit.Updater;
using UnityEngine;
using System.IO;
using System.Collections;
using naxokit.Screens;


namespace naxokit
{
    internal class naxokitDashboard : EditorWindow
    {
        bool SettingsOpen = false;
        bool CreditsOpen = false;
        bool AboutOpen = false;
        bool UpdateOpen = false;
        bool PremiumOpen = false;
        private Vector2 scrollPosition;

        [MenuItem("naxokit/Dashboard")]
        public static void ShowWindow() => GetWindow(typeof(naxokitDashboard));

        private async void OnEnable()
        {
            titleContent = new GUIContent("Dashboard");
            minSize = new Vector2(600, 400);

            //Loads the latest version from the server
            await naxokitUpdater.UpdateVersionData();

        }

        private void OnGUI()
        {
            if (naxokitUpdater.ServerVersionList == null || naxokitUpdater.LatestVersion == null || naxokitUpdater.LatestBetaVersion == null)
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Loading...", EditorStyles.boldLabel);
                }
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.BeginVertical();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.LabelField("naxokit", EditorStyles.boldLabel);
            
            var HeaderImages = new Hashtable()
            {
                {"Settings", Resources.Load("Settings") as Texture2D},
                {"Credits", Resources.Load("Credits") as Texture2D},
                {"About", Resources.Load("About") as Texture2D},
                {"Update", Resources.Load("Update") as Texture2D},
                {"Premium", Resources.Load("Premium") as Texture2D}

            };
            
            foreach (DictionaryEntry entry in HeaderImages)
            {
                var key = entry.Key;
                var value = entry.Value;
                if (key.ToString() == "Settings")
                {
                    SettingsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, SettingsOpen, 30f, 0, 0, 12f, 5f);
                    if (SettingsOpen)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            Settings.HandleSettingsOpend();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                if (key.ToString() == "Credits")
                {
                    CreditsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, CreditsOpen, 30f, 0, 0, 12f, 5f);
                    if (CreditsOpen)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            Credits.HandleCreditsOpend();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                if (key.ToString() == "About")
                {
                    AboutOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, AboutOpen, 30f, 0, 0, 12f, 5f);
                    if (AboutOpen)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            About.HandleAboutOpend();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                if (key.ToString() == "Update")
                {
                    UpdateOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, UpdateOpen, 30f, 0, 0, 12f, 5f);
                    if (UpdateOpen)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            Update.HandleUpdateOpend();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                if (key.ToString() == "Premium")
                {
                    PremiumOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, PremiumOpen, 30f, 0, 0, 12f, 5f);
                    if (PremiumOpen)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            Premium.HandlePremiumOpend();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            EditorGUILayout.LabelField("V" + naxokitUpdater.CurrentVersion.Replace(';', ' '), EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        private void NaxoLog(string message)
        {
            Debug.Log("<color=magenta>" + message + "</color>");
            Debug.Log("[naxokit]" + message);
        }
    }
}