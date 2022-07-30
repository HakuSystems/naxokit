using naxokit.Helpers.VRCSDK.Thumbnail;
using UnityEngine;

public class VRCThumbnailSelector : MonoBehaviour {
    private bool bAddScript = false;
    public Texture2D _texture;
    void Update(){
        if(false == bAddScript){
            GameObject obj = GameObject.Find("VRCCam");
            if(null != obj){
                bAddScript = true;
                obj.AddComponent<VRCThumbnailOverlay>();
                VRCThumbnailOverlay script = obj.GetComponent<VRCThumbnailOverlay>();
                if(null == script) return;
                script.enabled = false;
                script.SetTexture(_texture);
            }
        }
    }
}