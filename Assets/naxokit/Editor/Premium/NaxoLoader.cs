
using UnityEngine;
using UnityEditor;
using naxokit.Helpers.Logger;
using System;

namespace naxokit.Screens{
    public class NaxoLoader : EditorWindow {

        public static AssetBundle assetBundle;
        public static GameObject prefab;
        private static void ShowWindow() {
            var window = GetWindow<NaxoLoader>();
            window.Show();
        }
        private void OnEnable() {
            titleContent = new GUIContent("NaxoLoader");
            minSize = new Vector2(600, 300);
        }
        private void OnGUI() {
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            GUILayout.Button("naxokit", EditorStyles.toolbarButton);
            GUILayout.Button("Waiting for Drag and Drop Action", EditorStyles.toolbarButton);
            GUILayout.Button("NaxoLoader", EditorStyles.toolbarButton);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Box(".VRCA Asset Bundles only", EditorStyles.boldLabel);
            if(GUILayout.Button("Load AssetBundle", EditorStyles.toolbarButton)){
                var path = EditorUtility.OpenFilePanel("Select AssetBundle", "", "vrca");
                if(path.EndsWith(".vrca")) LoadAssetBundle(path);
                else{
                    EditorUtility.DisplayDialog("Error", path.ToString()+" is Not a valid VRCA file", "OK");
                }
            }
            Event evt = Event.current;
            if (evt.type == EventType.DragUpdated) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                evt.Use();
            }
            if (evt.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();
                foreach (string draggedObject in DragAndDrop.paths) {
                    if(draggedObject.EndsWith(".vrca")) LoadAssetBundle(draggedObject);
                    else{
                        EditorUtility.DisplayDialog("Error", draggedObject.ToString()+" is Not a valid VRCA file", "OK");
                    }
                }
                evt.Use();
            }
            GUILayout.EndHorizontal();

        }

        private void LoadAssetBundle(string path)
        {
            EditorUtility.DisplayProgressBar("Working", "Please Wait", 0f); // its not loading but fuck it :dance:
            try{
                if(assetBundle){
                    assetBundle.Unload(false);
                    DestroyImmediate(prefab);
                }
                assetBundle = AssetBundle.LoadFromFile(path);
                foreach(var obj in assetBundle.LoadAllAssets<GameObject>()){
                    prefab = (GameObject)Instantiate(obj);
                }
            }catch(Exception e){
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error", e.Message, "OK");
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
