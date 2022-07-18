using naxokit.DISCORDRPC;
using naxokit.Helpers;
using naxokit.Helpers.Configs;
using naxokit.Styles;
using UnityEditor;
using UnityEngine;

namespace naxokit.Screens
{
    public class Settings : EditorWindow
    {
        private static bool runOnce = false;

        public static bool isDiscordEnabled;
        public static void HandleSettingsOpend()
        {
            DrawLine.DrawHorizontalLine();
            //Handling PlayerPrefs
            GetCurrentPlayerPrefs();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {

                isDiscordEnabled = EditorGUILayout.Toggle("Discord RichPresence", isDiscordEnabled);
                if (GUILayout.Button("Hide Username"))
                    naxokitRPC.ChangeStateRPC();

            }
            EditorGUILayout.EndHorizontal();
            DrawLine.DrawHorizontalLine();

        }

        private static void GetCurrentPlayerPrefs()
        {
            if (!runOnce)
            {
                isDiscordEnabled = Converters.intToBool(PlayerPrefs.GetInt("rich_discord"));


                runOnce = true;
            }
        }

        public static void UpdateConfigs()
        {
            //currently only this config
            discord_rich.UpdateDiscordRichConfig(isDiscordEnabled);
        }
    }
}



