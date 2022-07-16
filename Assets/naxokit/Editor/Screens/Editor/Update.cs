using naxokit.Styles;
using naxokit.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class Update : EditorWindow
    {
        //Version selector
        public static List<VersionBaseINTERNDATA> versionList;
        public static string currentVersion;
        private static Vector2 scrollView;
        private static bool runOnce;
        private static string _searchString = "";

        public static void HandleUpdateOpend()
        {
            //User is on Latest Build
            DrawLine.DrawHorizontalLine();
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
                            else if (version.Version.Contains(_searchString))
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
                    EditorWindow.GetWindow<naxokitDashboard>().Close();
                    EditorWindow.GetWindow<naxokitDashboard>().Show();
                }

            }
            DrawLine.DrawHorizontalLine();
        }
    }
}
