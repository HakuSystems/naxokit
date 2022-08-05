using UnityEngine;
using UnityEditor;
//TODOO: This needs VRChat API Authentication.
//The unofficial API is only available on nuget
public class VRCFriendList : EditorWindow {
    
    public static void ShowWindow() {
        var window = GetWindow<VRCFriendList>();
        window.titleContent = new GUIContent("VRCFriendList");
        window.Show();
    }

    private void OnGUI() {
        
    }
}