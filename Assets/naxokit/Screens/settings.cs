using UnityEngine;
using UnityEditor;
using naxokit;
using naxokit.Styles;

namespace naxokit.Screens{
    public class Settings : EditorWindow{
        bool State = false;
        Texture2D texture2d;
        public Settings(string image){
            texture2d = Resources.Load(image) as Texture2D;
        }
        FoldoutTexture helper = new FoldoutTexture();
        State = helper.MakeTextureFoldout();
        /*{
            if(state){
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Coming soon...");
                }
                EditorGUILayout.EndVertical();
            }
        }*/
    }
}