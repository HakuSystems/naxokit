using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using naxokit;
using naxokit.Styles;
using naxokit.Updater;
using UnityEngine;

namespace Assets.naxokit.Editor
{
    internal class naxokitDashboard : EditorWindow
    {
        bool SettingsOpen = false; //ja 
        bool CreditsOpen = false;
        bool AboutOpen = false; //ja
        bool UpdateOpen = false;
        bool PremiumOpen = false;


        [MenuItem("naxokit/Dashboard")]
        public static void ShowWindow() => GetWindow(typeof(naxokitDashboard));

        private async void OnEnable()
        {
            titleContent = new GUIContent("Dashboard");
            minSize = new Vector2(600, 400);

            //Loads the latest version from the server
            await naxokitUpdater.UpdateVersionData();

        }

        private void OnGUI()
        {
            if (naxokitUpdater.ServerVersionList == null || naxokitUpdater.LatestVersion == null || naxokitUpdater.LatestBetaVersion == null)
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Loading...", EditorStyles.boldLabel);
                }
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("naxokit", EditorStyles.boldLabel);
                var settingsImage = Resources.Load("Settings") as Texture2D;
                var creditsImage = Resources.Load("Credits") as Texture2D;
                var aboutImage = Resources.Load("About") as Texture2D;
                var updateImage = Resources.Load("Update") as Texture2D;
                var premiumImage = Resources.Load("Premium") as Texture2D;
                
                
                PremiumOpen = FoldoutTexture.MakeTextureFoldout(premiumImage, PremiumOpen, 30f, 0, 0, 12f, 5f);
                if (PremiumOpen)
                {
                    /*
                     * indev
                    */
                }
                SettingsOpen = FoldoutTexture.MakeTextureFoldout(settingsImage, SettingsOpen, 30f, 0, 0, 12f, 5f);
                if (SettingsOpen)
                {
                    /*
                     * indev
                    */
                }
                AboutOpen = FoldoutTexture.MakeTextureFoldout(aboutImage, AboutOpen, 30f, 0, 0, 12f, 5f);
                if (AboutOpen)
                {
                    /*
                     * indev
                    */
                }
                CreditsOpen = FoldoutTexture.MakeTextureFoldout(creditsImage, CreditsOpen, 30f, 0, 0, 12f, 5f);
                if (CreditsOpen)
                {
                    var teamCreditsImage = Resources.Load("TeamCredits") as Texture2D;
                    DrawLine.DrawHorizontalLine(1);

                    //Adding Scale Fit thing todoo!!
                    var content = new GUIContent(teamCreditsImage);

                    EditorGUILayout.HelpBox(content);

                    DrawLine.DrawHorizontalLine(1);

                    EditorGUILayout.Space(10);


                }
                UpdateOpen = FoldoutTexture.MakeTextureFoldout(updateImage, UpdateOpen, 30f, 0, 0, 12f, 5f);
                if (UpdateOpen)
                {
                    /*
                     * indev
                    */
                }
                EditorGUILayout.LabelField("V"+ naxokitUpdater.CurrentVersion.Replace(';', ' '), EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndVertical();
            
        }
    }
}
