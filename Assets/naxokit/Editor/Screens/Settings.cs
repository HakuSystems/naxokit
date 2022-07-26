﻿using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Styles;
using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class Settings : EditorWindow
    {
        public static void HandleSettingsOpend()
        {
            DrawLine.DrawHorizontalLine();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                DrawLine.DrawHorizontalLine(1,Color.magenta);
                EditorGUILayout.LabelField("Discord Rich Presence", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                {
                    Config.Discordrpc_Enabled = EditorGUILayout.Toggle("Enabled", Config.Discordrpc_Enabled);
                    EditorGUILayout.TextField("Enable/Disable Requries Restart!", EditorStyles.centeredGreyMiniLabel);
                    Config.Discordrpc_Username = EditorGUILayout.Toggle("Username Shown", Config.Discordrpc_Username);
                }
                EditorGUILayout.EndHorizontal();
                DrawLine.DrawHorizontalLine(1,Color.magenta);
                EditorGUILayout.LabelField("OTHERSETTINGSHERE",EditorStyles.boldLabel);
            }
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("Logout", GUILayout.Width(70)))
            {

                naxoApiHelper.Logout();
                GetWindow<naxokitDashboard>().Close();
                GetWindow<naxokitDashboard>().Show();
            }

            DrawLine.DrawHorizontalLine();

        }


        public static void UpdateConfigsAndChangeRPC()
        {
            DiscordRPC.naxokitRPC.UpdateRPC();
            Config.UpdateConfig();
        }
    }
}


