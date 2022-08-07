using System.Runtime.Versioning;
using System.Net.Http.Headers;
using System.Diagnostics;
using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Styles;
using naxokit.Updater;
using System.Collections;
using UnityEditor;
using UnityEngine;
using naxokit.Helpers.Logger;
using naxokit.Screens;

//FIXME: Somehow the Foldouts are not sized correctly. The foldout is too small.
//when UpdateOpen foldout is Active then the foldout is resized to the size of the content.
//we need to resize every foldout so every foldout has the same size.

namespace naxokit
{
    public class naxokitDashboard : EditorWindow
    {
        public bool VRCToolsOpen = false;
        bool SettingsOpen = false;
        bool CreditsOpen = false;
        bool UpdateOpen = false;
        bool _isPlaying = false;
        public bool ToolsOpen = false;
        bool PremiumOpen = false;
        private static bool LoginOpen = true;
        private static bool SignUpOpen;
        private static bool PlayingOpen;
        private Vector2 scrollPosition;
        private bool userIsUptoDate = false;
        private bool hasSDK = true;
        public static bool finallyLoggedIn = false;
        public static bool savePasswordLocally = false;
        public static string passStatus = "";
        private static string usernameInput;
        private static string passwordInput;
        private static string emailInput;
        private static string redeemCode;


        [MenuItem("naxokit/Dashboard")]
        public static void ShowWindow() => GetWindow(typeof(naxokitDashboard));

        private void CheckSDK()
        {
#if VRC_SDK_VRCSDK3
                naxoLog.Log("CheckSDK", "VRCSDK found"); //works fine.
#else
            naxoLog.LogWarning("CheckSDK", "VRCSDK not found");
            hasSDK = false;
#endif
        }

        private async void OnEnable()
        {
            titleContent = new GUIContent("Dashboard");
            minSize = new Vector2(600, 700);

            //check if user has the VRCSDK installed
            CheckSDK();
            //Loads the latest version from the server
            await naxokitUpdater.UpdateVersionData();
            if (naxokitUpdater.CompareCurrentVersionWithLatest())
                userIsUptoDate = true;

        }
        private void Update()
        {
            LoginOpen = true; //Prefending the user from closing the Foldout while logging in.
        }
        private void OnDestroy()
        {
            Settings.UpdateConfigsAndChangeRPC();
            AssetDatabase.Refresh();
        }
        public static void SetFinallyLoggedIn(bool isLoggedIn)
        {
            if (!naxoApiHelper.IsLoggedInAndVerified())
                finallyLoggedIn = false;
            else
                finallyLoggedIn = isLoggedIn;
        }
        private static Hashtable ToolNames()
        {
            Hashtable toolsName = new Hashtable(){
                {"Settings", Resources.Load("Settings") as Texture2D},
                {"Credits", Resources.Load("Credits") as Texture2D},
                {"Premium", Resources.Load("Premium") as Texture2D},
                {"PlayMode", Resources.Load("PlayMode")as Texture2D },
                {"VRCTools", Resources.Load("VRCTools")as Texture2D},
                {"Tools", Resources.Load("Tools")as Texture2D}
            };
            return toolsName;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {
                    if (naxokitUpdater.ServerVersionList == null
                                            && naxokitUpdater.LatestVersion == null
                                            && naxokitUpdater.LatestBetaVersion == null)
                    {
                        EditorGUILayout.LabelField("Please wait while we load the latest version data.");
                        return;
                    }

                    #region Login and Signup
                    EditorGUILayout.BeginVertical();
                    {
                        if (!naxoApiHelper.IsUserLoggedIn())
                        {
                            LoginOpen = FoldoutTexture.MakeTextureFoldout(Resources.Load("Login") as Texture2D, LoginOpen, 30f, 0, 0, 12f, 5f);
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
                            SignUpOpen = FoldoutTexture.MakeTextureFoldout(Resources.Load("SignUp") as Texture2D, SignUpOpen, 30f, 0, 0, 12f, 5f);
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
                    EditorGUILayout.EndVertical();


                    #endregion

                    #region  All Tools aka Navigation
                    if (finallyLoggedIn || naxoApiHelper.IsLoggedInAndVerified())
                    {
                        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
                        GUILayout.Button(naxoApiHelper.User.Username, EditorStyles.boldLabel);
                        if (!hasSDK)
                            if (GUILayout.Button("Install VRCSDK", EditorStyles.toolbarButton))
                                GetWindow(typeof(VRCSDKInstaller));
                        //check if user is in playmode
                        if (EditorApplication.isPlaying)
                        {
                            if (GUILayout.Button("Stop PlayMode", EditorStyles.toolbarButton))
                            {
                                EditorApplication.isPlaying = false;
                                _isPlaying = false;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Start PlayMode", EditorStyles.toolbarButton))
                            {
                                EditorApplication.isPlaying = true;
                                _isPlaying = true;
                            }
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Logout", EditorStyles.toolbarButton))
                        {
                            naxoApiHelper.Logout();
                            GetWindow<naxokitDashboard>().Close();
                            GetWindow<naxokitDashboard>().Show();
                            return;
                        }
                        GUILayout.Button(naxoApiHelper.User.Permission.ToString(), EditorStyles.toolbarButton);
                        GUILayout.EndHorizontal();

                        EditorGUILayout.BeginVertical();
                        foreach (DictionaryEntry tool in ToolNames())
                        {
                            if (naxoApiHelper.IsUserLoggedIn())
                            {
                                switch (tool.Key.ToString())
                                {
                                    case "Settings":
                                        SettingsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, SettingsOpen, 30f, 0, 0, 12f, 5f);
                                        if (SettingsOpen)
                                        {
                                            EditorGUILayout.BeginVertical();
                                            {
                                                Settings.HandleSettingsOpend();
                                            }
                                            EditorGUILayout.EndVertical();
                                        }
                                        break;
                                    case "Credits":
                                        CreditsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, CreditsOpen, 30f, 0, 0, 12f, 5f);
                                        if (CreditsOpen)
                                        {
                                            EditorGUILayout.BeginVertical();
                                            {
                                                Credits.HandleCreditsOpend();
                                            }
                                            EditorGUILayout.EndVertical();
                                        }
                                        break;
                                    case "Premium":
                                        if ((naxoApiHelper.User.IsPremium)) //TODO: Find a better solution for Premium Checks
                                        {
                                            PremiumOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, PremiumOpen, 30f, 0, 0, 12f, 5f);
                                            if (PremiumOpen)
                                            {
                                                EditorGUILayout.BeginVertical();
                                                {
                                                    naxokit.Screens.Premium.HandlePremiumOpend();
                                                }
                                                EditorGUILayout.EndVertical();
                                            }
                                        }
                                        break;
                                    case "PlayMode":
                                        if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
                                        {
                                            PlayingOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, PlayingOpen, 30f, 0, 0, 12f, 5f);
                                            if (PlayingOpen)
                                            {
                                                EditorGUILayout.BeginVertical();
                                                {
                                                    naxokit.Screens.PlayMode.HandlePlayModeOpend();
                                                }
                                                EditorGUILayout.EndVertical();
                                            }
                                        }
                                        break;
                                    case "VRCTools":
                                        if (hasSDK) //hasSDK, CheckSDK
                                        {
                                            VRCToolsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, VRCToolsOpen, 30f, 0, 0, 12f, 5f);
                                            if (VRCToolsOpen)
                                            {
                                                EditorGUILayout.BeginVertical();
                                                {
                                                    VRCTools.HandleVRCToolsOpend();
                                                }
                                                EditorGUILayout.EndVertical();
                                            }
                                        }
                                        break;
                                    case "Tools":
                                        ToolsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, ToolsOpen, 30f, 0, 0, 12f, 5f);
                                        if (ToolsOpen)
                                        {
                                            EditorGUILayout.BeginVertical();
                                            {
                                                naxokit.Screens.naxoTools.HandleToolsOpend();
                                            }
                                            EditorGUILayout.EndVertical();
                                        }
                                        break;
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
                            UpdateOpen = FoldoutTexture.MakeTextureFoldout(Resources.Load("Update") as Texture2D, UpdateOpen, 30f, 0, 0, 12f, 5f);
                            if (UpdateOpen)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    DrawLine.DrawHorizontalLine();
                                    naxokit.Screens.Update.HandleUpdateOpend();
                                    DrawLine.DrawHorizontalLine();

                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.LabelField("V" + naxokitUpdater.CurrentVersion.Replace(';', ' '), EditorStyles.centeredGreyMiniLabel);
                        }
                    }
                    #endregion
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
