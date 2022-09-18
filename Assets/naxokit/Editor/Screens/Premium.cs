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
            GUILayout.Label("Thanks for the Support on Patreon!", EditorStyles.largeLabel);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("EasySearch"))
                {
                    GetWindow<EasySearch>().Show();
                }
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
                if (GUILayout.Button("NaxoLoader"))
                {
                    GetWindow<NaxoLoader>().Show();
                }
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
                if (GUILayout.Button("BackupManager"))
                {
                    GetWindow<BackupManager>().Show();
                }
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
                
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
