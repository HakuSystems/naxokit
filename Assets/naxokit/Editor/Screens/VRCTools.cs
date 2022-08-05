using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using naxokit.Styles;
using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class VRCTools : EditorWindow
    {
        public static void HandleVRCToolsOpend()
        {
            DrawLine.DrawHorizontalLine();
            EditorGUILayout.LabelField("VRCTools", EditorStyles.boldLabel);
            DrawLine.DrawHorizontalLine(1, Color.magenta);
            EditorGUILayout.LabelField("VRCFriendList", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Open VRC Friend List"))
                {
                    VRCFriendList.ShowWindow();
                }
                EditorGUILayout.LabelField("Open VRC Friend List!", EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndHorizontal();
            DrawLine.DrawHorizontalLine(1, Color.magenta);
            DrawLine.DrawHorizontalLine();
        }
    }
}
