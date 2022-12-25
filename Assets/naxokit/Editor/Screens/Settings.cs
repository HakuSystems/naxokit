using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Styles;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace naxokit.Screens
{
    public class Settings : EditorWindow
    {

        public static void HandleSettingsOpend()
        {
            DiscordSettings();
            GUILayout.Space(10);
            AutoSaverSettings();
            GUILayout.Space(10);
            if (!VRChatSDKCheck.FoundSDK())
            {
                PlaymodeSettings();
            }
            else
            {
                GUILayout.Label("PlaymodeSettings are disabled because you have the VRChat SDK installed.", EditorStyles.boldLabel);
            }
            GUILayout.Space(10);
            DefaultPathSettings();
            GUILayout.Space(10);
            UpdatesSettings();
        }

        private static void DefaultPathSettings()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Change Default Path"))
                    Config.DefPath = null;
                if(GUILayout.Button("Open"))
                    if (Config.DefPath != null)
                        Process.Start(Config.DefPath);
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void UpdatesSettings()
        {
            EditorGUILayout.BeginHorizontal();
            {
                Config.CheckForUpdates = EditorGUILayout.Toggle("Check for Updates", Config.CheckForUpdates);
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
            }
            EditorGUILayout.EndHorizontal();
            
        }

        private static void PlaymodeSettings()
        {
                EditorGUILayout.BeginHorizontal();
                {
                    Config.NaxoPlayModeTools_Enabled = EditorGUILayout.Toggle("NaxoPlayMode Tools", Config.NaxoPlayModeTools_Enabled);
                    EditorGUILayout.LabelField("Additional Settings for Playmode", new GUIStyle(EditorStyles.textField) {normal = {textColor = Color.yellow}});
                    if (GUILayout.Button("?", GUILayout.Width(20)))
                    {
                        //TODO: Add Link to the Documentation on Youtube
                    }
                
                }
                EditorGUILayout.EndHorizontal();
            
            
        }

        private static void AutoSaverSettings()
        {
            EditorGUILayout.BeginHorizontal();
            {
                Config.SceneAutosaver_Enabled = EditorGUILayout.Toggle("Scene Autosaver", Config.SceneAutosaver_Enabled);
                EditorGUILayout.LabelField("Only saves in Edit Mode!", new GUIStyle(EditorStyles.textField) {normal = {textColor = Color.yellow}});
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
                
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DiscordSettings()
        {
            EditorGUILayout.LabelField("Discord Rich Presence", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                Config.Discordrpc_Enabled = EditorGUILayout.Toggle("Enabled", Config.Discordrpc_Enabled);
                EditorGUILayout.LabelField("Unity Requries Restart!", new GUIStyle(EditorStyles.textField) {normal = {textColor = Color.yellow}});
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                Config.Discordrpc_Username = EditorGUILayout.Toggle("Username Shown", Config.Discordrpc_Username);
                if (Config.Discordrpc_Username)
                    EditorGUILayout.LabelField("Shown", new GUIStyle(EditorStyles.textField) {normal = {textColor = Color.green}});
                else
                    EditorGUILayout.LabelField("Hidden", new GUIStyle(EditorStyles.textField) {normal = {textColor = Color.red}});
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    //TODO: Add Link to the Documentation on Youtube
                }
            }
            EditorGUILayout.EndHorizontal();
            
        }


        public static void UpdateConfigsAndChangeRPC()
        {
            DiscordRPC.naxokitRPC.UpdateRPC();
            Config.UpdateConfig();
        }
    }
}



