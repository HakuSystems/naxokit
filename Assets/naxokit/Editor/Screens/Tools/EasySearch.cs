using System.Security.Principal;
using System.Diagnostics;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using naxokit.Helpers.Logger;
using naxokit.Premium;
using UnityEditor.UIElements;

namespace naxokit.Screens
{
    public class EasySearch : EditorWindow
    {

        private string _searchString = "";
        private string _endsWithString = "unitypackage";
        private static int _sliderLeftValue = 10;


        private static Vector2 _scrollPosition;

        public static void ShowWindow()
        {
            var window = GetWindow<EasySearch>();
            window.titleContent = new GUIContent("EasySearch");
            window.Show();
        }
        private void OnEnable()
        {
            //check if the results will show in the editor window
            if (Everything.Everything_GetMatchPath())
            {
                Everything.Everything_SetMatchCase(false);
            }

            titleContent = new GUIContent("EasySearch");
            minSize = new Vector2(600, 300);
        }
        private void OnGUI()
        {

            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            GUILayout.Button("naxokit", EditorStyles.toolbarButton);
            GUILayout.Button("EasySearch", EditorStyles.toolbarButton);
            _searchString = GUILayout.TextField(_searchString, GUI.skin.FindStyle("ToolbarSeachTextField"),
                GUILayout.Width(450));
            if (GUILayout.Button("X", EditorStyles.toolbarButton))
            {
                _searchString = string.Empty;
                GUI.FocusControl(null);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EndsWith:", EditorStyles.centeredGreyMiniLabel);
            _endsWithString = EditorGUILayout.TextField(_endsWithString, EditorStyles.toolbarButton);
            _sliderLeftValue = EditorGUILayout.IntSlider(_sliderLeftValue, 1, 1000);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Search", EditorStyles.toolbarButton))
            {
                if (_endsWithString.StartsWith("."))
                    _endsWithString = _endsWithString.Replace(".", "");
                if (Process.GetProcessesByName("Everything").Length != 0) FillList();
                else
                {
                    if (EditorUtility.DisplayDialog("nanoSDK", "Search Everything isnt Running please make sure to run it.",
                            "Okay", "Install")) Close();
                    else InstallEverything();
                }
            }

            EditorGUILayout.EndHorizontal();


            if (_results == null) return;

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(600));
            var resultCount = 0;
            var hash = new HashSet<string>();
            foreach (var result in _results)
            {
                resultCount++;
                if (hash.Contains(result.Filename)) continue;
                hash.Add(result.Filename);

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    if (result.Filename.EndsWith(".unitypackage"))
                    {
                        GUILayout.Label(result.Filename);
                        if (GUILayout.Button("Add to MassImporter", EditorStyles.toolbarButton))
                        {
                            MassImporter.AddToMassImporter(result.Path);
                        }
                        if (GUILayout.Button("Import", EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            AssetDatabase.ImportPackage(result.Path, true);
                        }
                    }
                    else
                    {
                        GUILayout.Label(result.Filename);
                        if (GUILayout.Button("(try to)Open", EditorStyles.toolbarButton, GUILayout.Width(80)))
                        {
                            Process.Start(result.Path);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);
            }

            GUILayout.EndScrollView();

        }
        private readonly List<Everything.Result> _results = new List<Everything.Result>();

        private void InstallEverything()
        {
            var voidToolsURL = "https://www.voidtools.com/downloads/";
            Application.OpenURL(voidToolsURL);

        }

        private void FillList()
        {
            _results.Clear();
            _results.AddRange(Everything.Search($"{_searchString} endwith:.{_endsWithString}", _sliderLeftValue));
        }
    }
}
