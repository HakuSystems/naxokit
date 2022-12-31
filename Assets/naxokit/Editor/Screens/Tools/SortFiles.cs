using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace naxokit.Editor.Screens.Tools
{
    public class SortFiles : EditorWindow
    {
        private static readonly Dictionary<string, Type> assetTypes = AssetDictionary.AssetTypes;
        private static Vector2 scrollPos;
        private static bool[] foldouts;
        private static string[] assetTypeNames;

        //[MenuItem("MENUITEM/MENUITEMCOMMAND")]
        private static void ShowWindow()
        {
            var window = GetWindow<SortFiles>();
            window.titleContent = new GUIContent("SortFiles");
            window.Show();
        }

        private void OnEnable()
        {
            minSize = new Vector2(700, 400);
        }

        private void OnGUI()
        {
            if (foldouts == null)
            {
                foldouts = new bool[assetTypes.Count];
                assetTypeNames = new string[assetTypes.Count];
                var i = 0;
                foreach (var assetType in assetTypes)
                {
                    assetTypeNames[i] = assetType.Key;
                    i++;
                }
            }

            EditorGUILayout.LabelField("This window allows you to sort files in your Unity project by asset type.");
            EditorGUILayout.HelpBox("Use this feature with caution. Sorting files may cause your project to break.",
                MessageType.Warning);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (var i = 0; i < assetTypes.Count; i++)
            {
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], assetTypeNames[i], true);
                if (!foldouts[i]) continue;
                // Display a button for sorting files of the current asset type
                if (GUILayout.Button("Sort " + assetTypeNames[i] + " Files"))
                    FilesSortner(assetTypes[assetTypeNames[i]]);
            }

            EditorGUILayout.EndScrollView();
        }

        private static void FilesSortner(Type assetType)
        {
            var filePaths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);

            
            foreach (var filePath in filePaths)
            {
                
                var fileExtension = Path.GetExtension(filePath);
                var fileName = Path.GetFileName(filePath);

                
                var targetFolder = "";
                switch (fileExtension)
                {
                    case ".anim":
                        targetFolder = "Animations";
                        break;
                    case ".controller":
                        targetFolder = "Controllers";
                        break;
                    case ".overrideController":
                        targetFolder = "Controllers";
                        break;
                    case ".audio":
                        targetFolder = "Audio";
                        break;
                    case ".avatar":
                        targetFolder = "Avatars";
                        break;
                    case ".cubemap":
                        targetFolder = "Cubemaps";
                        break;
                    case ".mat":
                        targetFolder = "Materials";
                        break;
                    case ".mesh":
                        targetFolder = "Meshes";
                        break;
                    case ".prefab":
                        targetFolder = "Prefabs";
                        break;
                    case ".shader":
                        targetFolder = "Shaders";
                        break;
                    case ".tga":
                        targetFolder = "Textures";
                        break;
                    case ".png":
                        targetFolder = "Textures";
                        break;
                    case ".jpg":
                        targetFolder = "Textures";
                        break;
                    case ".bmp":
                        targetFolder = "Textures";
                        break;
                    case ".gif":
                        targetFolder = "Textures";
                        break;
                }

                if (!string.IsNullOrEmpty(targetFolder))
                {
                    var targetDirectory = Path.Combine(Application.dataPath, $"Assets/{targetFolder}");
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    var targetPath = Path.Combine(Application.dataPath, $"Assets/{targetFolder}/{fileName}");
                    File.Move(filePath, targetPath);
                }
            }
        }
    }
}