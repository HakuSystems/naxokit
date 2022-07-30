using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class VRCSDKInstaller : EditorWindow
    {
        [MenuItem("Naxokit/VRCSDK Installer")]
        private void OnEnable()
        {
            titleContent = new GUIContent("VRCSDK Installer");
            minSize = new Vector2(500, 300);
        }
        public static void OnGUI()
        {
        }
    }
}