using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace naxokit
{
    public class AssetDictionary : MonoBehaviour
    {
        public static Dictionary<string, System.Type> AssetTypes = new Dictionary<string, System.Type>()
        {
            { "AnimationClip", typeof(AnimationClip) },
            { "AnimatorController", typeof(AnimatorController) },
            { "AnimatorOverrideController", typeof(AnimatorOverrideController) },
            { "AudioClip", typeof(AudioClip) },
            { "Avatar", typeof(Avatar) },
            { "Cubemap", typeof(Cubemap) },
            { "Material", typeof(Material) },
            { "Mesh", typeof(Mesh) },
            { "Prefab", typeof(GameObject) },
            { "Shader", typeof(Shader) },
            { "Texture2D", typeof(Texture2D) }
        };
    }
}