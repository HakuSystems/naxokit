using System;
using System.Collections.Generic;
using naxokit.Editor.Screens.Tools;
using naxokit.Screens.Tools;
using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class naxoTools : EditorWindow
    {
        public static void HandleToolsOpend()
        {
            //Todo: Make youtube videos - and add them to the tools
            var tools = new Dictionary<string, string>
            {
                { "AudioSourceVolumeControl", "https:///naxokit.com/discord" },
                { "MassImporter", "https:///naxokit.com/discord" },
                { "PresetManager", "https:///naxokit.com/discord" },
                { "EasySearch", "https:///naxokit.com/discord" },
                { "BackupManager", "https:///naxokit.com/discord" },
                { "SortFiles", "https:///naxokit.com/discord" }
            };

            EditorGUILayout.BeginHorizontal();
            foreach (var tool in tools)
            {
                if (!GUILayout.Button(tool.Key)) continue;
                var windowType = Type.GetType(tool.Key);
                if (windowType != null)
                    windowType.GetMethod("ShowWindow")?.Invoke(null, null);
                else
                    switch (tool.Key)
                    {
                        case "PresetManager":
                            GetWindow<PresetManager>().Show();
                            break;
                        case "EasySearch":
                            GetWindow<EasySearch>().Show();
                            break;
                        case "SortFiles":
                            GetWindow<SortFiles>().Show();
                            break;
                    }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}