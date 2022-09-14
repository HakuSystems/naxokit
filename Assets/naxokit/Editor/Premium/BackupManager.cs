using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using naxokit.Styles;
using naxokit.Helpers.Logger;
using System;
using System.Diagnostics;
using naxokit.Helpers.Configs;
using naxokit.Screens;

public class BackupManager : EditorWindow
{
    private Vector2 scrollPosition;
    static List<string> assets = new List<string>(); //list of assets to backup its always empty until AssetDatabase.ExportPackage is called

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

        EditorGUILayout.LabelField("Backup Destination", EditorStyles.boldLabel);
        Config.BackupManager_BackupFolder_Selected = EditorGUILayout.TextField(Config.BackupManager_BackupFolder_Selected);
        EditorGUILayout.BeginHorizontal();
        {
            Config.BackupManager_SaveinProjectFolder_Enabled = EditorGUILayout.Toggle("Save in Project Folder", Config.BackupManager_SaveinProjectFolder_Enabled);
            if (!Directory.Exists(Config.BackupManager_BackupFolder_Selected))
                Directory.CreateDirectory(Config.BackupManager_BackupFolder_Selected);

            if (!Config.BackupManager_SaveinProjectFolder_Enabled)
            {
                if (GUILayout.Button("Select Folder on Computer"))
                    Config.BackupManager_BackupFolder_Selected = EditorUtility.OpenFolderPanel("Select Folder on Computer", "", "");
            }
        }
        EditorGUILayout.EndHorizontal();
        Config.BackupManager_SaveAsUnitypackage_Enabled = EditorGUILayout.Toggle("Save as Unitypackage", Config.BackupManager_SaveAsUnitypackage_Enabled);
        Config.BackupManager_DeleteOldBackups_Enabled = EditorGUILayout.Toggle("Clean Disk Space", Config.BackupManager_DeleteOldBackups_Enabled);
        Config.BackupManager_AutoBackup_Enabled = EditorGUILayout.Toggle("Auto Backup", Config.BackupManager_AutoBackup_Enabled);


        if (Config.BackupManager_SaveinProjectFolder_Enabled)
            Config.BackupManager_BackupFolder_Selected = Application.dataPath + "/naxokit/backups";
        if (Config.BackupManager_BackupFolder_Selected == "" || !Directory.Exists(Config.BackupManager_BackupFolder_Selected)) return;

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Backup", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
                CreateBackup(Config.BackupManager_SaveAsUnitypackage_Enabled, Config.BackupManager_DeleteOldBackups_Enabled);
            if (GUILayout.Button("Delete All Backups", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
            {
                if (EditorUtility.DisplayDialog("BackupManager", "Are you sure you want to delete all backups?", "Yes", "No"))
                {
                    if (Directory.Exists(Config.BackupManager_BackupFolder_Selected))
                    {
                        var directories = Directory.GetDirectories(Config.BackupManager_BackupFolder_Selected);
                        foreach (var directory in directories)
                        {
                            Directory.Delete(directory, true);
                            File.Delete(directory + ".meta");
                        }
                        naxoLog.Log("BackupManager", "All backups deleted");
                    }
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
            if (GUILayout.Button("Open Backup Folder", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
            {
                if (Directory.Exists(Config.BackupManager_BackupFolder_Selected))
                    Process.Start(Config.BackupManager_BackupFolder_Selected);
            }
        }
        EditorGUILayout.EndHorizontal();
        DrawLine.DrawHorizontalLine(1, Color.magenta);

        EditorGUILayout.LabelField("BackupList", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        {
            EditorGUILayout.BeginVertical();
            {
                foreach (var folder in Directory.GetDirectories(Config.BackupManager_BackupFolder_Selected))
                {
                    if (folder.Contains("naxokit"))
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (Config.BackupManager_DeleteOldBackups_Enabled)
                            {
                                GUI.color = Color.red;
                                GUILayout.Label("Deletion Pending", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString()));
                                GUI.color = Color.white;
                            }
                            EditorGUILayout.LabelField(folder.Split('/').Last(), EditorStyles.boldLabel);
                            if (GUILayout.Button("Open", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
                            {

                                if (Directory.Exists(folder))
                                    Process.Start(folder);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

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
        if (!Directory.Exists(Config.BackupManager_BackupFolder_Selected))
            Directory.CreateDirectory(Config.BackupManager_BackupFolder_Selected);

        var results = 0f;
        if (Directory.Exists(Config.BackupManager_BackupFolder_Selected))
        {
            EditorUtility.DisplayProgressBar("BackupManager", "Creating Backup", results);
            var backupName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var backupPath = Config.BackupManager_BackupFolder_Selected + "/" + backupName;
            Directory.CreateDirectory(backupPath);
            var files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            if (!_saveAsUnitypackage)
            {
                foreach (var file in files)
                {
                    var relativePath = file.Replace(Application.dataPath, "");
                    var destination = backupPath + relativePath.Replace("\\", "/");
                    var destinationDirectory = Path.GetDirectoryName(destination);
                    if (!Directory.Exists(destinationDirectory))
                        Directory.CreateDirectory(destinationDirectory);
                    File.Copy(file, destination);
                    results++;
                    EditorUtility.DisplayProgressBar("BackupManager", "Creating Backup", results / files.Length);
                }
            }
            else
            {
                var packagePath = backupPath + "/" + backupName + ".unitypackage";
                AssetDatabase.ExportPackage(assets.ToArray(), packagePath, ExportPackageOptions.Recurse);
            }

            if (_deleteOldBackups)
            {
                var directories = Directory.GetDirectories(Config.BackupManager_BackupFolder_Selected, "*", SearchOption.TopDirectoryOnly);
                foreach (var directory in directories)
                {
                    if (directory.Contains(backupName)) continue;
                    Directory.Delete(directory, true);
                    File.Delete(directory + ".meta");
                    results++;
                    EditorUtility.DisplayProgressBar("BackupManager", "Deleting Old Backups", results / directories.Length);
                }
            }


            naxoLog.Log("BackupManager", "Backup created");
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

    }

    private string GetLastBackupDate()
    {
        if (Directory.Exists(Config.BackupManager_BackupFolder_Selected))
        {
            var directories = Directory.GetDirectories(Config.BackupManager_BackupFolder_Selected);
            if (directories.Length > 0)
            {
                var lastBackup = directories[directories.Length - 1];
                var lastBackupDate = Directory.GetCreationTime(lastBackup);
                if (lastBackup.Contains("naxokit"))
                    return lastBackupDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        return "Never";
    }
}