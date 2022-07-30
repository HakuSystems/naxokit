using System.IO;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

namespace naxokit.Helpers.VRCSDK.Thumbnail{
    [CustomEditor(typeof(VRCThumbnailOverlayEditor))]
    public class VRCThumbnailOverlayEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if(GUILayout.Button("Image")){
                GameObject obj = GameObject.Find("VRCCam");
                if(null == obj) return;
                VRCThumbnailOverlay script = obj.GetComponent<VRCThumbnailOverlay>();
                if(null == script) return;
                string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");
                if(string.IsNullOrEmpty(path)) return;
                if(path.Length > 0){
                    Texture2D tex = new Texture2D(1,1);
                    if(null != tex){
                        tex.LoadImage(File.ReadAllBytes(path));
                        tex.filterMode = FilterMode.Point;
                        script.SetTexture(tex);
                        script.enabled = true;
                    }
                }
            }
        }
    }
}
