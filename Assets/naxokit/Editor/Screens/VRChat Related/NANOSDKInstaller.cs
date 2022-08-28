using UnityEngine;
using UnityEditor;
//TODO: Fetch the VRC SDK version from https://api.vrchat.cloud/api/1/config look for "downloadUrls" there should be 
//a download url for the latest version of the SDK.
public class NANOSDKInstaller : EditorWindow
{
    public static void ShowWindow()
    {
        var window = GetWindow<NANOSDKInstaller>();
        window.titleContent = new GUIContent("NANOSDKInstaller");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("it is Recommended to uninstall naxokit. since some scripts may not work after install", EditorStyles.wordWrappedLabel);
        }
        EditorGUILayout.EndHorizontal();
    }
}