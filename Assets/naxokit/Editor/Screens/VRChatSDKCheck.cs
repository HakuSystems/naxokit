using System;
using System.IO;
using System.Linq;
using naxokit.Helpers.Configs;
using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class VRChatSDKCheck : EditorWindow
    {
        //[MenuItem("MENUITEM/MENUITEMCOMMAND")]
        public static void ShowWindow()
        {
            var window = GetWindow<VRChatSDKCheck>();
            window.titleContent = new GUIContent("VRChatSDKCheck");
            window.Show();
        }

        private void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.red;
            if (!FoundSDK())
            {
                CheckIfPlayModeToolsExist();
                Close();
                return;
            }
            EditorGUILayout.LabelField("PLEASE WAIT A SECOND", style);
            RemoveVRChatNotCompatible();

        }

        private void CheckIfPlayModeToolsExist()
        {
            var file = Directory.GetFiles(Application.dataPath, "SaveHierarchyOnClick.cs", SearchOption.AllDirectories).FirstOrDefault();
            if (file != null) return;
            BackupManager.CreateBackup(Config.BackupManager_SaveAsUnitypackage_Enabled,
                Config.BackupManager_DeleteOldBackups_Enabled);
            Updater.naxokitUpdater.DownloadVersion();
        }

        private void RemoveVRChatNotCompatible()
        {
            var file = Directory.GetFiles(Application.dataPath, "SaveHierarchyOnClick.cs", SearchOption.AllDirectories).FirstOrDefault();
            if(file != null)
                File.Delete(file);
            AssetDatabase.Refresh();
            Close();
        }

        public static bool FoundSDK()
        {
            var files = Directory.GetFiles(Application.dataPath, "VRCSDK3A-Editor.dll", SearchOption.AllDirectories);
            return files.Length != 0;
        }
    }
}