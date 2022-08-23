using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using naxokit.Styles;
using naxokit.Helpers.Logger;
using System;
using System.Diagnostics;

public class BackupManager : EditorWindow
{
    private Vector2 scrollPosition;
    private string backupFolder = "";
    private bool saveinProjectFolder = true;
    private bool saveAsUnitypackage = true;
    List<string> assets = new List<string>(); //list of assets to backup its always empty until AssetDatabase.ExportPackage is called

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

    private void OnGUI()
    {
        DrawLine.DrawHorizontalLine();
        GUILayout.Button("Backup Your UnityProject", EditorStyles.toolbarButton);
        DrawLine.DrawHorizontalLine();
        EditorGUILayout.LabelField("Backup", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Backup Destination", EditorStyles.boldLabel);
        backupFolder = EditorGUILayout.TextField(backupFolder);
        EditorGUILayout.BeginHorizontal();
        {
            saveinProjectFolder = EditorGUILayout.Toggle("Save in Project Folder", saveinProjectFolder);
            if (!saveinProjectFolder)
            {
                if (GUILayout.Button("Select Folder on Desktop"))
                    backupFolder = EditorUtility.OpenFolderPanel("Select Folder on Desktop", "", "");
            }
            saveAsUnitypackage = EditorGUILayout.Toggle("Save as Unitypackage", saveAsUnitypackage);
        }
        EditorGUILayout.EndHorizontal();

        if (saveinProjectFolder)
            backupFolder = Application.dataPath + "/naxokit/backups";

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Backup", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
            {
                if (!Directory.Exists(backupFolder))
                    Directory.CreateDirectory(backupFolder);

                if (Directory.Exists(backupFolder))
                {
                    var backupName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                    var packageName = "naxokit-backup-" + backupName;
                    var backupPath = backupFolder + "/" + packageName;
                    if (Directory.Exists(backupPath))
                        Directory.Delete(backupPath, true);
                    Directory.CreateDirectory(backupPath);
                    var files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
                    if (!saveAsUnitypackage)
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
                    naxoLog.Log("BackupManager", "Backup created in " + backupPath);
                }
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Delete All Backups", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
            {
                if (EditorUtility.DisplayDialog("BackupManager", "Are you sure you want to delete all backups?", "Yes", "No"))
                {
                    if (Directory.Exists(backupFolder))
                    {
                        var directories = Directory.GetDirectories(backupFolder);
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
                if (Directory.Exists(backupFolder))
                    Process.Start(backupFolder);
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Thanks for the Support on Patreon!", EditorStyles.centeredGreyMiniLabel);

    }

    private string GetLastBackupDate()
    {
        if (Directory.Exists(backupFolder))
        {
            var directories = Directory.GetDirectories(backupFolder);
            if (directories.Length > 0)
            {
                var lastBackup = directories[directories.Length - 1];
                var lastBackupDate = Directory.GetCreationTime(lastBackup);
                return lastBackupDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        return "Never";
    }
}