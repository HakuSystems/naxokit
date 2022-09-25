using UnityEngine;
using UnityEditor;
using naxokit.Helpers.Logger;
using System.Collections.Generic;
using naxokit.Styles;
using System.IO;
using System;
using System.Linq;
using naxokit.Helpers.Auth;

public class MassImporter : EditorWindow
{
    private readonly List<string> _paths = new List<string>();
    private Vector2 _scrollPosition;

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
        _paths.Clear();
    }
    private void Update()
    {
        foreach (var path in _paths.Where(path => !File.Exists(path)))
        {
            _paths.Remove(path);
            naxoLog.Log("MassImporter", "File does not exist anymore: " + path);
            break;
        }
    }
    private void OnGUI()
    {
        var evt = Event.current;
        if (evt.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            evt.Use();
        }
        if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            foreach (var draggedObject in DragAndDrop.paths)
            {
                _paths.Add(draggedObject);
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
                    _paths.Add(filePanelPath);
                    naxoLog.Log("MassImporter", "Added " + filePanelPath);
                }
            }
            DrawLine.DrawHorizontalLine();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                foreach (var path in _paths)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Remove", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.ButtonMid.ToString())))
                        {
                            _paths.Remove(path);
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
                if (_paths.Count != 0 && GUILayout.Button("Import All"))
                {
                    foreach (var path in _paths)
                    {
                        AssetDatabase.ImportPackage(path, false);
                    }
                    _paths.Clear();
                    naxoLog.Log("MassImporter", "Imported all packages");
                }
                if (_paths.Count != 0 && GUILayout.Button("Remove All")) _paths.Clear();
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();
    }

    internal static void AddToMassImporter(string path)
    {
        var window = GetWindow<MassImporter>();
        window._paths.Add(path);
    }
}