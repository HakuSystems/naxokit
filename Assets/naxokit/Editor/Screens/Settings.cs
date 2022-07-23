using naxokit.Helpers.Auth;
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
            EditorGUILayout.BeginHorizontal();
            {
                Config.discordrpc_Enabled = EditorGUILayout.Toggle("Discord RichPresence", Config.discordrpc_Enabled);
                Config.discordrpc_Username = EditorGUILayout.Toggle("Username Shown", Config.discordrpc_Username);
                UpdateConfigsAndChangeRPC();

            }
            EditorGUILayout.EndHorizontal();
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



