using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using naxokit.Styles;
using naxokit.Screens;
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
            EditorGUILayout.BeginHorizontal();
            {
                //TODO: VRC Related Tools
            }
            EditorGUILayout.EndHorizontal();
            DrawLine.DrawHorizontalLine();
        }
    }
}
