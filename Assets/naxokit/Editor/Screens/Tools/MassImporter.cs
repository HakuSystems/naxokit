using UnityEngine;
using UnityEditor;
using naxokit.Helpers.Logger;
using System.Collections.Generic;
using naxokit.Styles;
using System.IO;
using System;
using naxokit.Helpers.Auth;

public class MassImporter : EditorWindow
{
    private List<string> paths = new List<string>();
    private Vector2 scrollPosition;

    //[MenuItem("naxokitDevelopment/MassImporter")]
    public static void ShowWindow()
    {
        var window = GetWindow<MassImporter>();
        window.titleContent = new GUIContent("MassImporter");
        window.Show();
    }
    private void OnEnable()
    {
        minSize = new Vector2(300, 300);
        paths.Clear();
    }
    private void Update()
    {
        foreach (var path in paths)
        {
            if (!File.Exists(path))
            {
                paths.Remove(path);
                naxoLog.Log("MassImporter", "File does not exist anymore: " + path);
                break;
            }
        }
    }
    private void OnGUI()
    {
        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            evt.Use();
        }
        if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            foreach (string draggedObject in DragAndDrop.paths)
            {
                paths.Add(draggedObject);
                naxoLog.Log("MassImporter", "Added " + draggedObject);
            }
            evt.Use();
        }


        EditorGUILayout.BeginVertical();
        {
            DrawLine.DrawHorizontalLine();
            GUILayout.Button("Waiting for Drag and Drop Action", EditorStyles.toolbarButton);
            if (GUILayout.Button("Search on Computer"))
            {
                var filePanelPath = EditorUtility.OpenFilePanel("Select UnityPackage", "", "unitypackage");
                if (filePanelPath.EndsWith(".unitypackage"))
                {
                    paths.Add(filePanelPath);
                    naxoLog.Log("MassImporter", "Added " + filePanelPath);
                }
            }
            DrawLine.DrawHorizontalLine();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                foreach (var path in paths)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Remove", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.ButtonMid.ToString())))
                        {
                            paths.Remove(path);
                            naxoLog.Log("MassImporter", "Removed " + path);
                            break;
                        }
                        EditorGUILayout.LabelField(path, new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.HeaderLabel.ToString()));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            {
                if (paths.Count != 0 && GUILayout.Button("Import All"))
                {
                    foreach (var path in paths)
                    {
                        AssetDatabase.ImportPackage(path, false);
                    }
                    paths.Clear();
                    naxoLog.Log("MassImporter", "Imported all packages");
                }
                if (paths.Count != 0 && GUILayout.Button("Remove All")) paths.Clear();
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();
    }

    internal static void AddToMassImporter(string path)
    {
        var window = GetWindow<MassImporter>();
        window.paths.Add(path);
    }
}