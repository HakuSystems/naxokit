using UnityEngine;
using UnityEditor;
using naxokit.Styles;

namespace naxokit.Screens{
    public class PlayMode : EditorWindow {

        public GameObject _thumbnailOverlayGameObject;
        public static void HandlePlayModeOpend(){
            DrawLine.DrawHorizontalLine();
            EditorGUILayout.LabelField("Play Mode", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                #if VRC_SDK_VRCSDK3
                //if(naxokitDashboard.hasSDK)
                //{
                    
                //}
                
                #endif
            }
            EditorGUILayout.EndVertical();
            DrawLine.DrawHorizontalLine();
        }
    }
}
