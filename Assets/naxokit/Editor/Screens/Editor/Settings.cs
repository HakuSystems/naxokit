using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using naxokit.Styles;
using Discord;

namespace naxokit.Screens
{
    public class Settings : EditorWindow
    {
        public static bool enableDiscord = true;
        public Settings()
        {
        }
        public static void HandleSettingsOpend()
        {

            //checkbox enableDiscord
            enableDiscord = EditorGUILayout.Toggle("Enable Discord Rich Presence", enableDiscord);
            if (!enableDiscord)
                enableDiscord = false;

        }
        public static bool DiscordRichPresence() { return enableDiscord; }
    }
}



