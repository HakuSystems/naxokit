using System.Timers;
using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
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
                EditorGUILayout.LabelField("Discord Rich Presence", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                {
                    Config.Discordrpc_Enabled = EditorGUILayout.Toggle("Enabled", Config.Discordrpc_Enabled);
                    EditorGUILayout.TextField("Unity Requries Restart!", EditorStyles.centeredGreyMiniLabel);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    Config.Discordrpc_Username = EditorGUILayout.Toggle("Username Shown", Config.Discordrpc_Username);
                    if (Config.Discordrpc_Username)
                        EditorGUILayout.TextField("Shown", EditorStyles.centeredGreyMiniLabel);
                    else
                        EditorGUILayout.TextField("Hidden", EditorStyles.centeredGreyMiniLabel);
                }
                EditorGUILayout.EndHorizontal();


                GUILayout.Space(5);

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Scene Autosaver ", EditorStyles.boldLabel);
                    EditorGUILayout.TextField("Only saves in Edit Mode!", EditorStyles.centeredGreyMiniLabel);
                    Config.SceneAutosaver_Enabled = EditorGUILayout.Toggle("Enabled", Config.SceneAutosaver_Enabled);
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(5);

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("NaxoPlayMode Tools", EditorStyles.boldLabel);
                    EditorGUILayout.TextField("Additional Settings for Playmode", EditorStyles.centeredGreyMiniLabel);
                    Config.NaxoPlayModeTools_Enabled = EditorGUILayout.Toggle("Enabled", Config.NaxoPlayModeTools_Enabled);
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(5);


            }
            EditorGUILayout.EndVertical();

            DrawLine.DrawHorizontalLine();

        }


        public static void UpdateConfigsAndChangeRPC()
        {
            DiscordRPC.naxokitRPC.UpdateRPC();
            Config.UpdateConfig();
            naxoLog.Log("Settings", "Configs updated!");
        }
    }
}



