using System;
using System.Collections.Generic;
using naxokit.Helpers.Models;
using naxokit.Styles;
using naxokit.Updater;
using UnityEditor;
using UnityEngine;

namespace naxokit
{
    public class SwitchVersion : EditorWindow
    {
        public static void ShowWindow() => GetWindow(typeof(SwitchVersion));
        private static Vector3 _scrollPosition;

        private void OnEnable()
        {
            titleContent = new GUIContent("SwitchVersion");
            naxokitUpdater.GetVersionList();
            minSize = new Vector2(500, 300);
            
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            ListVersions();
            EditorGUILayout.EndScrollView();
        }

        private static void ListVersions()
        {
            if (VersionDataList == null) return;
            foreach (var version in VersionDataList)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("V"+version.Version, new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.PreLabel.ToString()));
                    EditorGUILayout.LabelField("Release Date: "+version.CommitDate, new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.ErrorLabel.ToString()));
                    switch (version.Branch)
                    {
                        case NaxoVersionData.BranchType.Release:
                            EditorGUILayout.LabelField("Release Version", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.HelpBox.ToString()));
                            break;
                        case NaxoVersionData.BranchType.Beta:
                            EditorGUILayout.LabelField("Beta Version", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.HelpBox.ToString()));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    if (GUILayout.Button("Switch",
                            new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.LargeButton.ToString())))
                    {
                        naxokitUpdater.DownloadVersion(version.Url, version.Version, version.Branch);
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }

        public static List<NaxoVersionData> VersionDataList { get; set; }
    }
}