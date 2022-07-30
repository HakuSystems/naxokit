using UnityEngine;

namespace naxokit.Helpers.VRCSDK.Thumbnail{
    public class VRCThumbnailOverlay : MonoBehaviour {
        private Texture2D _texture;
        private Material _material;

        public void SetTexture(Texture2D texture){
            if(null == texture) return;
            _texture = texture;
            if(null == _material){
                Shader overlayShader = Shader.Find("naxokit/ThumbnailOverlay");
                if(null == overlayShader) return;
                _material = new Material(overlayShader);
                if(null == _material) return;

            }
            _material.SetTexture("_Overlay", _texture);
        }
        void OnRenderImage(RenderTexture src, RenderTexture dest) {
            if(null == _material) return;
            _material.SetVector("_UV_Transform", new Vector4(1, 0, 0, 1));
            _material.SetTexture("_Overlay", _texture);
            Graphics.Blit(src, dest, _material);
        }
    }
}
    
