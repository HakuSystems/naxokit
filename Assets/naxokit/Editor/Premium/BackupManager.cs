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
        Settings.UpdateConfigsAndChangeRPC();
        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        DrawLine.DrawHorizontalLine();
        GUILayout.Button("Backup Your UnityProject", EditorStyles.toolbarButton);
        DrawLine.DrawHorizontalLine();
        EditorGUILayout.LabelField("Backup", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Backup Destination", EditorStyles.boldLabel);
        Config.BackupFolder_Selected = EditorGUILayout.TextField(Config.BackupFolder_Selected);
        EditorGUILayout.BeginHorizontal();
        {
            Config.SaveinProjectFolder_Enabled = EditorGUILayout.Toggle("Save in Project Folder", Config.SaveinProjectFolder_Enabled);
            if (!Config.SaveinProjectFolder_Enabled)
            {
                if (GUILayout.Button("Select Folder on Computer"))
                    Config.BackupFolder_Selected = EditorUtility.OpenFolderPanel("Select Folder on Computer", "", "");
            }
        }
        EditorGUILayout.EndHorizontal();
        Config.SaveAsUnitypackage_Enabled = EditorGUILayout.Toggle("Save as Unitypackage", Config.SaveAsUnitypackage_Enabled);
        Config.DeleteOldBackups_Enabled = EditorGUILayout.Toggle("Clean Disk Space", Config.DeleteOldBackups_Enabled);
        Config.AutoBackup_Enabled = EditorGUILayout.Toggle("Auto Backup", Config.AutoBackup_Enabled);


        if (Config.SaveinProjectFolder_Enabled)
            Config.BackupFolder_Selected = Application.dataPath + "/naxokit/backups";
        if (Config.BackupFolder_Selected == "" || !Directory.Exists(Config.BackupFolder_Selected)) return;

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Backup", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
                CreateBackup(Config.SaveAsUnitypackage_Enabled, Config.DeleteOldBackups_Enabled);
            if (GUILayout.Button("Delete All Backups", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
            {
                if (EditorUtility.DisplayDialog("BackupManager", "Are you sure you want to delete all backups?", "Yes", "No"))
                {
                    if (Directory.Exists(Config.BackupFolder_Selected))
                    {
                        var directories = Directory.GetDirectories(Config.BackupFolder_Selected);
                        foreach (var directory in directories)
                        {
                            Directory.Delete(directory, true);
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
                if (Directory.Exists(Config.BackupFolder_Selected))
                    Process.Start(Config.BackupFolder_Selected);
            }
        }
        EditorGUILayout.EndHorizontal();
        DrawLine.DrawHorizontalLine(1, Color.magenta);

        EditorGUILayout.LabelField("BackupList", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        {
            EditorGUILayout.BeginVertical();
            {
                foreach (var folder in Directory.GetDirectories(Config.BackupFolder_Selected))
                {
                    if (folder.Contains("naxokit"))
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (Config.DeleteOldBackups_Enabled)
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
        if (!Directory.Exists(Config.BackupFolder_Selected))
            Directory.CreateDirectory(Config.BackupFolder_Selected);

        if (Directory.Exists(Config.BackupFolder_Selected))
        {
            if (_deleteOldBackups)
            {
                var directories = Directory.GetDirectories(Config.BackupFolder_Selected);
                foreach (var directory in directories)
                {
                    Directory.Delete(directory, true);
                }
            }

            var backupName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var packageName = "naxokit-backup-" + backupName;
            var backupPath = Config.BackupFolder_Selected + "/" + packageName;
            if (Directory.Exists(backupPath))
                Directory.Delete(backupPath, true);
            Directory.CreateDirectory(backupPath);
            var files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            if (!_saveAsUnitypackage)
            {
                foreach (var file in files)
                {
                    var relativePath = file.Replace(Application.dataPath, "");
                    var destination = backupPath + relativePath;
                    var destinationDirectory = Path.GetDirectoryName(destination);
                    if (!Directory.Exists(destinationDirectory))
                        Directory.CreateDirectory(destinationDirectory);
                    File.Copy(file, destination);
                }
            }
            else
            {
                var packagePath = backupPath + "/" + packageName + ".unitypackage";
                AssetDatabase.ExportPackage(assets.ToArray(), packagePath, ExportPackageOptions.Recurse);
            }
            naxoLog.Log("BackupManager", "Backup created");
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

    }

    private string GetLastBackupDate()
    {
        if (Directory.Exists(Config.BackupFolder_Selected))
        {
            var directories = Directory.GetDirectories(Config.BackupFolder_Selected);
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