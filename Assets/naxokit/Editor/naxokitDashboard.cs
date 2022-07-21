﻿using naxokit.Helpers.Configs;
using naxokit.Screens;
using naxokit.Screens.Auth;
using naxokit.Styles;
using naxokit.Updater;
using System.Collections;
using UnityEditor;
using UnityEngine;


namespace naxokit
{
    internal class naxokitDashboard : EditorWindow
    {
        bool SettingsOpen = false;
        bool CreditsOpen = false;
        bool UpdateOpen = false;
        bool PremiumOpen = false;
        private static bool LoginOpen = true;
        private static bool SignUpOpen;
        private Vector2 scrollPosition;
        private bool userIsUptoDate = false;
        public static bool finallyLoggedIn = false;
        public static bool savePasswordLocally = false;
        public static string passStatus = "";


        private static string usernameInput;
        private static string passwordInput;
        private static string emailInput;
        private static string redeemCode;

        [MenuItem("naxokit/Dashboard")]
        public static void ShowWindow() => GetWindow(typeof(naxokitDashboard));

        private async void OnEnable()
        {
            titleContent = new GUIContent("Dashboard");
            minSize = new Vector2(600, 700);


            //Loads the latest version from the server
            await naxokitUpdater.UpdateVersionData();
            if (naxokitUpdater.CompareCurrentVersionWithLatest())
                userIsUptoDate = true;

        }
        private void OnDestroy()
        {
            Settings.UpdateConfigs();
            AssetDatabase.Refresh();
        }
        public static void SetFinallyLoggedIn(bool isLoggedIn)
        {
            if (!naxoApiHelper.IsLoggedInAndVerified())
                finallyLoggedIn = false;
            else
                finallyLoggedIn = isLoggedIn;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.LabelField("naxokit", EditorStyles.boldLabel);
                if (!finallyLoggedIn)
                    EditorGUILayout.LabelField("You have to login to use naxokit", EditorStyles.centeredGreyMiniLabel);



                var HeaderImages = new Hashtable()
                    {
                            {"Settings", Resources.Load("Settings") as Texture2D},
                            {"Credits", Resources.Load("Credits") as Texture2D},
                            {"Update", Resources.Load("Update") as Texture2D},
                            {"Premium", Resources.Load("Premium") as Texture2D},
                            {"Login", Resources.Load("Login")as Texture2D },
                            {"Signup",Resources.Load("Signup")as Texture2D }

                        };

                if (naxoApiHelper.IsUserLoggedIn())
                {
                    if (!naxoApiHelper.IsLoggedInAndVerified())
                    {
                        EditorGUILayout.LabelField("You are logged in but not verified", EditorStyles.centeredGreyMiniLabel);
                        DrawLine.DrawHorizontalLine();
                        EditorGUILayout.BeginHorizontal();
                        {
                            redeemCode = EditorGUILayout.TextField("License Key", redeemCode);
                            if (GUILayout.Button("?", GUILayout.Width(20)))
                            {
                                if (EditorUtility.DisplayDialog("Login", "To receive your License, you have to join our discord server!", "Lead me there"))
                                {
                                    Application.OpenURL("https://naxokit.com/discord");
                                }
                            }
                        }
                        if (GUILayout.Button("Redeem"))
                        {
                            if (string.IsNullOrEmpty(redeemCode))
                                EditorUtility.DisplayDialog("naxokitDashboard", "License Key cant be Empty", "Okay");
                            else
                                naxoApiHelper.RedeemLicense(redeemCode);
                        }
                        EditorGUILayout.EndHorizontal();
                        DrawLine.DrawHorizontalLine();
                    }
                }


                foreach (DictionaryEntry entry in HeaderImages)
                {
                    var key = entry.Key;
                    var value = entry.Value;
                    if (!naxoApiHelper.IsUserLoggedIn())
                    {
                        if (key.ToString() == "Login")
                        {
                            LoginOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, LoginOpen, 30f, 0, 0, 12f, 5f);
                            if (LoginOpen)
                            {
                                DrawLine.DrawHorizontalLine();
                                EditorGUILayout.LabelField("Login");
                                usernameInput = EditorGUILayout.TextField("Username", usernameInput);
                                passwordInput = EditorGUILayout.PasswordField("Password", passwordInput);
                                EditorGUILayout.BeginHorizontal();
                                {
                                    savePasswordLocally = EditorGUILayout.Toggle("Save Password Locally", savePasswordLocally);
                                    EditorGUILayout.LabelField(passStatus, EditorStyles.centeredGreyMiniLabel);
                                    passStatus = "no Password Saved";
                                    if (auth_api.Config.Password != null)
                                    {
                                        passStatus = "Password available";
                                        savePasswordLocally = true;
                                        passwordInput = naxoApiHelper.GetSavedPassword();
                                        if (GUILayout.Button("Clear Password"))
                                        {
                                            auth_api.Config.Password = null;
                                            passwordInput = null;
                                            usernameInput = null;
                                            auth_api.Save();
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                if (GUILayout.Button("Login"))
                                {
                                    if (string.IsNullOrEmpty(usernameInput) || string.IsNullOrEmpty(passwordInput))
                                        EditorUtility.DisplayDialog("Login", "Credentials cant be Empty", "Okay");
                                    else
                                        naxoApiHelper.Login(usernameInput, passwordInput);
                                }
                                DrawLine.DrawHorizontalLine();
                            }
                        }
                        if (key.ToString() == "Signup")
                        {
                            SignUpOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, SignUpOpen, 30f, 0, 0, 12f, 5f);
                            if (SignUpOpen)
                            {
                                DrawLine.DrawHorizontalLine();
                                usernameInput = EditorGUILayout.TextField("Username", usernameInput);
                                passwordInput = EditorGUILayout.PasswordField("Password", passwordInput);
                                emailInput = EditorGUILayout.TextField("Email", emailInput);
                                EditorGUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button("Generate Strong Password"))
                                    {
                                        passwordInput = naxoApiHelper.ApiGenerateStrongPassword();
                                    }
                                    if (GUILayout.Button("Create Account"))
                                    {
                                        if (string.IsNullOrEmpty(usernameInput) || string.IsNullOrEmpty(passwordInput) || string.IsNullOrEmpty(emailInput))
                                            EditorUtility.DisplayDialog("SignUp", "Credentials cant be Empty", "Okay");
                                        else
                                            naxoApiHelper.SignUp(usernameInput, passwordInput, emailInput);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                DrawLine.DrawHorizontalLine();
                            }
                        }
                    }
                    if (finallyLoggedIn && naxoApiHelper.IsLoggedInAndVerified())
                    {
                        if (naxokitUpdater.ServerVersionList == null || naxokitUpdater.LatestVersion == null || naxokitUpdater.LatestBetaVersion == null)
                        {
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("Loading...");
                            EditorGUILayout.EndVertical();
                            return;
                        }
                        if (key.ToString() == "Settings")
                        {
                            SettingsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, SettingsOpen, 30f, 0, 0, 12f, 5f);
                            if (SettingsOpen)
                            {
                                EditorGUILayout.BeginVertical();
                                Settings.HandleSettingsOpend();
                                EditorGUILayout.EndVertical();
                            }
                        }
                        if (key.ToString() == "Credits")
                        {
                            CreditsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, CreditsOpen, 30f, 0, 0, 12f, 5f);
                            if (CreditsOpen)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    Credits.HandleCreditsOpend();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        if (key.ToString() == "Update")
                        {
                            if (userIsUptoDate)
                            {
                                UpdateOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, UpdateOpen, 30f, 0, 0, 12f, 5f);
                                if (UpdateOpen)
                                {
                                    EditorGUILayout.BeginVertical();
                                    {
                                        Update.HandleUpdateOpend();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }
                        if (key.ToString() == "Premium")
                        {
                            PremiumOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)value, PremiumOpen, 30f, 0, 0, 12f, 5f);
                            if (PremiumOpen)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    Premium.HandlePremiumOpend();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                    }
                }
                if (!userIsUptoDate)
                {
                    DrawLine.DrawHorizontalLine();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Update Available", EditorStyles.boldLabel);
                        if (GUILayout.Button("Update", EditorStyles.miniButton, GUILayout.Width(100)))
                            naxokitUpdater.DeleteAndDownloadAsync();

                        if (naxokitUpdater.LatestVersion != null)
                            EditorGUILayout.LabelField("Version: " + naxokitUpdater.LatestVersion.Version, EditorStyles.centeredGreyMiniLabel);


                    }
                    EditorGUILayout.EndHorizontal();
                    DrawLine.DrawHorizontalLine();

                }
                else
                {
                    EditorGUILayout.LabelField("V" + naxokitUpdater.CurrentVersion.Replace(';', ' '), EditorStyles.centeredGreyMiniLabel);
                }

                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

    }
}