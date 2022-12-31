using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace naxokit.Editor.Screens.Tools
{
    public class SortFiles : EditorWindow
    {
        private static readonly Dictionary<string, Type> AssetTypes = AssetDictionary.AssetTypes;
        private static Vector2 _scrollPos;

        [FormerlySerializedAs("_foldouts")] [SerializeField]
        private bool[] foldouts;

        private static string[] _assetTypeNames;

        private static Dictionary<string, int> _assetTypeCounts;

        //[MenuItem("MENUITEM/MENUITEMCOMMAND")]
        private static void ShowWindow()
        {
            var window = GetWindow<SortFiles>();
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("SortFiles");
            minSize = new Vector2(700, 400);
            maxSize = new Vector2(700, 400);
        }

        private void OnGUI()
        {
            if (_assetTypeCounts == null)
            {
                _assetTypeCounts = new Dictionary<string, int>();
                _assetTypeNames = new string[AssetTypes.Count];
                foldouts = new bool[AssetTypes.Count];
                var i = 0;
                foreach (var assetType in AssetTypes)
                {
                    _assetTypeNames[i] = assetType.Key;
                    i++;
                }

                foreach (var assetTypeName in _assetTypeNames)
                {
                    var assetType = AssetTypes[assetTypeName];
                    var assets = AssetDatabase.FindAssets("t:" + assetType.Name);
                    _assetTypeCounts[assetTypeName] = 0;
                    foreach (var asset in assets)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(asset);
                        if (path.StartsWith("Assets")) _assetTypeCounts[assetTypeName]++;
                    }
                }
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("This window allows you to sort files in your Unity project by asset type. And when sorted, Files will be Stored in a new Folder something like AudioClip/yourAudioName.mp3", EditorStyles.wordWrappedLabel);
            EditorGUILayout.HelpBox("Use this feature with caution. Sorting files may cause your project to break.",
                MessageType.Warning);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            foreach (var f in _assetTypeNames)
            {
                var index = Array.IndexOf(_assetTypeNames, f);
                foldouts[index] =
                    EditorGUILayout.Foldout(foldouts[index], new GUIContent(f + " - " + _assetTypeCounts[f]));
                if (foldouts[index])
                {
                    var assetType = AssetTypes[f];
                    var assets = AssetDatabase.FindAssets("t:" + assetType.Name);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Asset Name", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Asset Path", EditorStyles.boldLabel);
                    EditorGUILayout.EndHorizontal();
                    foreach (var asset in assets)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(asset);
                        if (path.StartsWith("Assets"))
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(path));
                            EditorGUILayout.LabelField(path);
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    if (_assetTypeCounts[f] > 0)
                        if (GUILayout.Button("Move All"))
                            foreach (var asset in assets)
                            {
                                var path = AssetDatabase.GUIDToAssetPath(asset);
                                if (path.StartsWith("Assets"))
                                {
                                    var newPath = Path.Combine("Assets", f);
                                    if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
                                    AssetDatabase.Refresh();
                                    newPath = Path.Combine(newPath, Path.GetFileName(path));
                                    AssetDatabase.MoveAsset(path, newPath);
                                }
                            }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}