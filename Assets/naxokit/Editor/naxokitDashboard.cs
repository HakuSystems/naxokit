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

namespace Assets.naxokit.Editor
{
    internal class naxokitDashboard : EditorWindow
    {
        bool SettingsOpen = false; //ja 
        bool CreditsOpen = false;
        bool AboutOpen = false; //ja
        bool UpdateOpen = false;
        bool PremiumOpen = false;
        private Vector2 scrollPosition;

        //Version selector
        public List<VersionBaseINTERNDATA> versionList;
        public string currentVersion;
        private static Vector2 scrollView;
        private bool runOnce;
        private static string _searchString = "";

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
            {
                EditorGUILayout.LabelField("naxokit", EditorStyles.boldLabel);
                var settingsImage = Resources.Load("Settings") as Texture2D;
                var creditsImage = Resources.Load("Credits") as Texture2D;
                var aboutImage = Resources.Load("About") as Texture2D;
                var updateImage = Resources.Load("Update") as Texture2D;
                var premiumImage = Resources.Load("Premium") as Texture2D;
                
                
                PremiumOpen = FoldoutTexture.MakeTextureFoldout(premiumImage, PremiumOpen, 30f, 0, 0, 12f, 5f);
                if (PremiumOpen)
                {
                    /*
                     * indev
                    */
                }
                SettingsOpen = FoldoutTexture.MakeTextureFoldout(settingsImage, SettingsOpen, 30f, 0, 0, 12f, 5f);
                if (SettingsOpen)
                {
                    /*
                     * indev
                    */
                }
                AboutOpen = FoldoutTexture.MakeTextureFoldout(aboutImage, AboutOpen, 30f, 0, 0, 12f, 5f);
                if (AboutOpen)
                {
                    /*
                     * indev
                    */
                }
                CreditsOpen = FoldoutTexture.MakeTextureFoldout(creditsImage, CreditsOpen, 30f, 0, 0, 12f, 5f);
                if (CreditsOpen)
                {
                    var teamCreditsImage = Resources.Load("TeamCredits") as Texture2D;
                    DrawLine.DrawHorizontalLine(1);
                    var content = new GUIContent(teamCreditsImage);
                    EditorGUILayout.LabelField(content, GUILayout.Height(300));
                    DrawLine.DrawHorizontalLine(1);
                }
                UpdateOpen = FoldoutTexture.MakeTextureFoldout(updateImage, UpdateOpen, 30f, 0, 0, 12f, 5f);
                if (UpdateOpen)
                {
                    DrawLine.DrawHorizontalLine(1);
                    if (naxokitUpdater.CompareCurrentVersionWithLatest())
                    {
                        //User is on Latest Build
                        try
                        {
                            if (runOnce == false)
                            {
                                versionList = naxokitUpdater.ServerVersionList;
                                currentVersion = naxokitUpdater.LatestVersion.Version;
                                runOnce = true;
                            }
                            scrollView = EditorGUILayout.BeginScrollView(scrollView);
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    var updateImageDisplay = Resources.Load("LatestUpdateHeader") as Texture2D;
                                    var content = new GUIContent(updateImageDisplay);
                                    EditorGUILayout.LabelField(content, GUILayout.Height(140));
                                    
                                    EditorGUILayout.LabelField("Search Version", EditorStyles.boldLabel);
                                    EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
                                    {
                                        
                                        _searchString = GUILayout.TextField(_searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
                                        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
                                        {
                                            _searchString = "";
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    


                                    foreach (var version in versionList)
                                    {
                                        if (_searchString == "")
                                        {
                                            if (version.Version != currentVersion)
                                            {
                                                EditorGUILayout.BeginHorizontal();
                                                {
                                                    EditorGUILayout.LabelField(version.Version, EditorStyles.boldLabel);
                                                    if (GUILayout.Button("Install"))
                                                    {
                                                        naxokitUpdater.DeleteAndDownloadAsync(version.Version);
                                                    }
                                                }
                                                EditorGUILayout.EndHorizontal();

                                            }
                                        }
                                        else if(version.Version.Contains(_searchString))
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            {
                                                EditorGUILayout.LabelField(version.Version, EditorStyles.boldLabel);
                                                if (GUILayout.Button("Install"))
                                                {
                                                    naxokitUpdater.DeleteAndDownloadAsync(version.Version);
                                                }
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        
                                    }
                                    if (_searchString != "" && _searchString != "Current Version: " + versionList.Count)
                                    {
                                        //Bug #1: even when there is a result it shows that there is "no result"
                                        EditorGUILayout.LabelField("No results found for: " + _searchString, EditorStyles.boldLabel);
                                    }

                                }
                                EditorGUILayout.EndVertical();

                            }
                            EditorGUILayout.EndScrollView();

                        }
                        catch (Exception)
                        {
                            EditorGUILayout.LabelField("Error: Could not load version list", EditorStyles.boldLabel);
                            if (GUILayout.Button("Reload Window"))
                            {
                                Close();
                                ShowWindow();
                            }

                        }
                    }
                    else
                    {
                        //User is on a Old Build

                        var updateImageDisplay = Resources.Load("UpdateHeader") as Texture2D;
                        var content = new GUIContent(updateImageDisplay);
                        EditorGUILayout.LabelField(content, GUILayout.Height(140));
                        //idk yet
                    }
                    DrawLine.DrawHorizontalLine(1);
                    
                }
                EditorGUILayout.LabelField("V"+ naxokitUpdater.CurrentVersion.Replace(';', ' '), EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            
        }

        private void NaxoLog(string message)
        {
            Debug.Log("<color=magenta>" + message + "</color>");
            Debug.Log("[naxokit] " + message);

        }
    }
}
