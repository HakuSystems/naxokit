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
    public class Premium : EditorWindow
    {
        public static void HandlePremiumOpend()
        {
            DrawLine.DrawHorizontalLine();
            EditorGUILayout.LabelField("Premium", EditorStyles.boldLabel);
            if(GUILayout.Button("EasySearch"))
            {
                GetWindow<EasySearch>().Show();
            }
            if(GUILayout.Button("NaxoLoader"))
            {
                GetWindow<NaxoLoader>().Show();
            }
            DrawLine.DrawHorizontalLine();
        }
    }
}
