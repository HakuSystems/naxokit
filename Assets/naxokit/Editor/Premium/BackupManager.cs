using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using naxokit.Styles;
using naxokit.Helpers.Logger;
using System;
using System.Diagnostics;
using System.Globalization;
using naxokit.Helpers.Configs;
using naxokit.Screens;
using UnityEngine.Serialization;

public class BackupManager : EditorWindow
{
    private Vector2 _scrollPosition;
    [FormerlySerializedAs("Assets")] [SerializeField] private static List<string> assets = new List<string>(); //list of assets to backup its always empty until AssetDatabase.ExportPackage is called
    private static readonly string BackupPath = Config.DefPath + "/Backups/";

    //[MenuItem("naxokitDevelopment/BackupManager")]
    public static void ShowWindow()
    {
        var window = GetWindow<BackupManager>();
        window.titleContent = new GUIContent("BackupManager");
        window.Show();
    }
    private void OnEnable()
    {
        minSize = new Vector2(500, 200);
        if (!Directory.Exists(BackupPath))
            Directory.CreateDirectory(BackupPath);
        
    }
    private void OnDestroy()
    {
        Config.UpdateConfig();
        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        DrawLine.DrawHorizontalLine();
        GUILayout.Button("Backup Your UnityProject", EditorStyles.toolbarButton);
        DrawLine.DrawHorizontalLine();
        EditorGUILayout.LabelField("Backup", EditorStyles.boldLabel);
        Config.BackupManager_SaveAsUnitypackage_Enabled = EditorGUILayout.Toggle("Save as Unitypackage", Config.BackupManager_SaveAsUnitypackage_Enabled);
        Config.BackupManager_DeleteOldBackups_Enabled = EditorGUILayout.Toggle("Clean Disk Space", Config.BackupManager_DeleteOldBackups_Enabled);
        Config.BackupManager_AutoBackup_Enabled = EditorGUILayout.Toggle("Auto Backup", Config.BackupManager_AutoBackup_Enabled);

        

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Backup", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
                CreateBackup(Config.BackupManager_SaveAsUnitypackage_Enabled, Config.BackupManager_DeleteOldBackups_Enabled);
            if (GUILayout.Button("Delete All Backups", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
            {
                if (EditorUtility.DisplayDialog("BackupManager", "Are you sure you want to delete all backups?", "Yes", "No"))
                {
                    foreach (var file in Directory.GetFiles(BackupPath))
                    {
                        File.Delete(file);
                    }
                    naxoLog.Log("BackupManager", "All backups deleted");
                    AssetDatabase.Refresh();
                }

            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        DrawLine.DrawHorizontalLine(1, Color.magenta);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Last Backup: " + GetLastBackupDate(), EditorStyles.boldLabel);
            if(GUILayout.Button("Open Backup Folder", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
            {
                if (!Directory.Exists(BackupPath))
                    Directory.CreateDirectory(BackupPath);
                Process.Start(BackupPath);
            }
        }
        EditorGUILayout.EndHorizontal();
        DrawLine.DrawHorizontalLine(1, Color.magenta);

        EditorGUILayout.LabelField("BackupList", EditorStyles.boldLabel);
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        {
            EditorGUILayout.BeginVertical();
            {
                foreach (var file in Directory.GetFiles(BackupPath))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(Path.GetFileName(file));
                        if (Config.BackupManager_DeleteOldBackups_Enabled)
                        {
                            GUI.color = Color.red;
                            GUILayout.Label("Deletion Pending", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString()));
                            GUI.color = Color.white;
                        }
                        if (GUILayout.Button("Delete", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
                        {
                            if (EditorUtility.DisplayDialog("BackupManager", "Are you sure you want to delete this backup?", "Yes", "No"))
                            {
                                File.Delete(file);
                                naxoLog.Log("BackupManager", "Backup deleted");
                                AssetDatabase.Refresh();
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Label("Thanks for the Support on Patreon!", EditorStyles.centeredGreyMiniLabel);

    }

    public static void CreateBackup(bool _saveAsUnitypackage, bool _deleteOldBackups)
    {
        if (!Config.IsPremiumBoolSinceLastCheck)
            return;
        naxoLog.Log("BackupManager", "Creating Backup");

        var results = 0f;
        EditorUtility.DisplayProgressBar("BackupManager", "Creating Backup", results);
            var backupName = "naxokit "+ DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            Directory.CreateDirectory(BackupPath);
            var files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            if (!_saveAsUnitypackage)
            {
                foreach (var file in files)
                {
                    if (file.Contains(".meta")) continue;
                    var relativePath = file.Replace(Application.dataPath, "");
                    var destination = BackupPath + backupName + "/" + relativePath;
                    var destinationDir = Path.GetDirectoryName(destination);
                    if (!Directory.Exists(destinationDir))
                        Directory.CreateDirectory(destinationDir);
                    File.Copy(file, destination, true);
                    results += 1f / files.Length;
                    EditorUtility.DisplayProgressBar("BackupManager", "Creating Backup", results);
                    
                    
                }
            }
            else
            {
                EditorUtility.DisplayProgressBar("BackupManager", "Creating Unitypackage", results);
                AssetDatabase.ExportPackage(assets.ToArray(), BackupPath + backupName + ".unitypackage", ExportPackageOptions.Recurse);
                
            }

            if (_deleteOldBackups)
            {
                var backupFolders = Directory.GetDirectories(BackupPath);
                var backupFiles = Directory.GetFiles(BackupPath);
                var backupFoldersSorted = backupFolders.OrderBy(Directory.GetCreationTime).ToArray();
                var backupFilesSorted = backupFiles.OrderBy(File.GetCreationTime).ToArray();
                if (backupFoldersSorted.Length > 1)
                {
                    for (var i = 0; i < backupFoldersSorted.Length - 1; i++)
                    {
                        Directory.Delete(backupFoldersSorted[i], true);
                    }
                }
                if (backupFilesSorted.Length > 1)
                {
                    for (var i = 0; i < backupFilesSorted.Length - 1; i++)
                    {
                        if (backupFilesSorted[i].Contains(".unitypackage"))
                            File.Delete(backupFilesSorted[i]);
                    }
                }
            }


            naxoLog.Log("BackupManager", "Backup created");
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

    }

    private static string GetLastBackupDate()
    {
        //get last backup folder by date or unitypackage by date
        var backupFolders = Directory.GetDirectories(BackupPath);
        var backupFiles = Directory.GetFiles(BackupPath);
        var backupFoldersSorted = backupFolders.OrderBy(Directory.GetCreationTime).ToArray();
        var backupFilesSorted = backupFiles.OrderBy(File.GetCreationTime).ToArray();
        if (backupFoldersSorted.Length > 0)
        {
            return Directory.GetCreationTime(backupFoldersSorted.Last()).ToString("yyyy-MM-dd-HH-mm-ss");
        }
        else if (backupFilesSorted.Length > 0)
        {
            return File.GetCreationTime(backupFilesSorted.Last()).ToString("yyyy-MM-dd-HH-mm-ss");
        }
        else
        {
            return "No Backups";
        }
    }
}