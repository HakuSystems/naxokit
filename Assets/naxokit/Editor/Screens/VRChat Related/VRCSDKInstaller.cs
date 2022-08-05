using UnityEngine;
using UnityEditor;
//TODO: Fetch the VRC SDK version from https://api.vrchat.cloud/api/1/config look for "downloadUrls" there should be 
//a download url for the latest version of the SDK.
public class VRCSDKInstaller : EditorWindow {
    public static void ShowWindow() {
        var window = GetWindow<VRCSDKInstaller>();
        window.titleContent = new GUIContent("VRCSDKInstaller");
        window.Show();
    }

    private void OnGUI() {
        
    }
}