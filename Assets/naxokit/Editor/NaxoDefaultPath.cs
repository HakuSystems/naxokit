using System;
using System.IO;
using naxokit.Helpers.Configs;
using UnityEditor;
using UnityEngine;

namespace naxokit
{
    public class NaxoDefaultPath : EditorWindow
    {
        //[MenuItem("MENUITEM/MENUITEMCOMMAND")]
        public static void ShowWindow()
        {
            var window = GetWindow<NaxoDefaultPath>();
            window.titleContent = new GUIContent("Default Path");
            window.position = new Rect(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, 400, 150);
            window.Show();
        }

        private void OnEnable()
        {
            if (GetConfigValue()) return;
            maxSize = new Vector2(400, 150);
            minSize = new Vector2(400, 150);
        }

        private bool GetConfigValue()
        {
            if (Config.DefPath != null)
            {
                naxokitDashboard.ShowWindow();
                Close();
                return true;
            }

            var dashboard = (naxokitDashboard)GetWindow(typeof(naxokitDashboard));
            dashboard.Close();
            return false;
        }

        private void Update()
        {
            position = new Rect(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, 400, 150);
            if(Config.DefPath != null) Close();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Default Path", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Please Choose a Default Path for naxokit related stuff.\n" +
                                           "THIS IS NEEDED IN ORDER TO USE NAXOKIT!",
                    new GUIStyle(EditorStyles.foldout) {normal = {textColor = Color.green}});
                EditorGUILayout.Space();
                SaveInProject = EditorGUILayout.Toggle("Save In Project", SaveInProject);
                if (!SaveInProject)
                {
                    GUI.backgroundColor = Color.green;
                    if (!GUILayout.Button("Select Default Path..")) return;
                    var path = EditorUtility.OpenFolderPanel("Select Default Path", "", "");
                    if (string.IsNullOrEmpty(path)) return;
                    OverrideConfigPaths(path);
                    naxokitDashboard.ShowWindow();
                    Close();
                }
                GUI.backgroundColor = Color.red;
                EditorGUILayout.LabelField("NOT RECOMMENDED",
                    new GUIStyle(EditorStyles.toolbarButton) { normal = { textColor = Color.yellow } });
                if (!GUILayout.Button("Save in Project")) return;
                EditorGUILayout.LabelField("NOT RECOMMENDED",
                    new GUIStyle(EditorStyles.toolbar) { normal = { textColor = Color.yellow } });
                var projectPath = Application.dataPath + "/naxokit/DEFAULT";
                if (!Directory.Exists(projectPath))
                    Directory.CreateDirectory(projectPath);
                OverrideConfigPaths(projectPath);
                Config.UpdateConfig();
                naxokitDashboard.ShowWindow();
                Close();
                
                
            }
            EditorGUILayout.EndVertical();
        }

        private static void OverrideConfigPaths(string path)
        {
            Config.DefPath = path;
            var version = File.ReadAllText("Assets/naxokit/Version.txt");
            if (!File.Exists(Config.DefPath + "/naxoVersion/Version.txt"))
            {
                Directory.CreateDirectory(Config.DefPath + "/naxoVersion");
                File.WriteAllText(Config.DefPath + "/naxoVersion/Version.txt", version);
            }
            Config.UpdateConfig();
        }

        private bool SaveInProject { get; set; }
    }
}