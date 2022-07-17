using UnityEditor;
using UnityEngine;
using naxokit.Styles;
using naxokit.Helpers;
using naxokit.Helpers.Configs;

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
                     
            isDiscordEnabled = EditorGUILayout.Toggle("Discord RichPresence", isDiscordEnabled);

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



