using System;
using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Styles;
using naxokit.Updater;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using naxokit.Helpers.Logger;
using naxokit.Helpers.Models;
using naxokit.Screens;

namespace naxokit
{
    public class naxokitDashboard : EditorWindow
    {
        public bool VRCToolsOpen = false;
        bool SettingsOpen = false;
        bool CreditsOpen = false;
        public bool ToolsOpen = false;
        bool PremiumOpen = false;
        private static bool LoginOpen = true;
        private static bool SignUpOpen;
        private static bool PlayingOpen;
        private Vector2 scrollPosition;
        public static bool UserIsUptoDate = false;
        private bool hasSDK = true;
        public static bool finallyLoggedIn = false;
        public static bool savePasswordLocally = false;
        public static string passStatus = "";
        private static string usernameInput;
        private static string passwordInput;
        private static string emailInput;
        private static string redeemCode;
        private Task _task;


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

        private void OnLostFocus()
        {
            Settings.UpdateConfigsAndChangeRPC();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Dashboard");
            minSize = new Vector2(1000, 300);
            Focus();
            //check if user has the VRCSDK installed
            CheckSDK();
            _task = naxokitUpdater.CheckForUpdates();
            
            //keep always down in OnEnable never move it up
            if (Config.DefPath != null) return;
            NaxoDefaultPath.ShowWindow();
            Close();
            //keep always down in OnEnable never move it up
            
        }
        private void Update()
        {
            LoginOpen = true; //Prefending the user from closing the Foldout while logging in.
            if (Config.DefPath != null) return;
            NaxoDefaultPath.ShowWindow();
            Close();
        }
        public static void SetFinallyLoggedIn(bool isLoggedIn) => finallyLoggedIn = naxoApiHelper.IsLoggedInAndVerified() && isLoggedIn;
        
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
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(EditorGUIUtility.currentViewWidth));
                {
                    #region Login and Signup
                    EditorGUILayout.BeginVertical();
                    {
                        if (!naxoApiHelper.IsUserLoggedIn())
                        {
                            LoginOpen = FoldoutTexture.MakeTextureFoldout(Resources.Load("Login") as Texture2D, LoginOpen);
                            if (LoginOpen)
                            {
                                DrawLine.DrawHorizontalLine();
                                EditorGUILayout.LabelField("Login");
                                usernameInput = EditorGUILayout.TextField("Username", usernameInput);
                                passwordInput = EditorGUILayout.PasswordField("Password", passwordInput);
                                var termsOfService = new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.ScriptText.ToString());
                                EditorGUILayout.BeginHorizontal();
                                {
                                    savePasswordLocally = EditorGUILayout.Toggle("Save Password Locally", savePasswordLocally);
                                    EditorGUILayout.LabelField(passStatus, EditorStyles.centeredGreyMiniLabel);
                                    passStatus = "no Password Saved";
                                    if (Config.Password != "")
                                    {
                                        passStatus = "Password available";
                                        savePasswordLocally = true;
                                        passwordInput = naxoApiHelper.GetSavedPassword();
                                        if (GUILayout.Button("Clear Password"))
                                        {
                                            Config.Password = null;
                                            passwordInput = null;
                                            usernameInput = null;
                                            Config.UpdateConfig();
                                        }
                                    }

                                    if (termsOfService == null) throw new ArgumentNullException(nameof(termsOfService));
                                    termsOfService.richText = true;
                                    EditorGUILayout.LabelField("By logging in you agree to our ");
                                    if (GUILayout.Button("Terms of Service", termsOfService))
                                        Application.OpenURL("https://naxokit.com/terms-of-service.html");
                                    
                                }
                                EditorGUILayout.EndHorizontal();

                                if (GUILayout.Button("Login"))
                                {
                                    if (string.IsNullOrEmpty(usernameInput) || string.IsNullOrEmpty(passwordInput))
                                        EditorUtility.DisplayDialog("Login", "Credentials cant be Empty", "Okay");
                                    else
                                        naxoApiHelper.Login(usernameInput, passwordInput);
                                }
                            }
                            SignUpOpen = FoldoutTexture.MakeTextureFoldout(Resources.Load("SignUp") as Texture2D, SignUpOpen);
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
                            EditorGUILayout.LabelField("You need to redeem a license key.", EditorStyles.centeredGreyMiniLabel);
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
                    if (finallyLoggedIn)
                    {
                        GUILayout.BeginHorizontal(GUI.skin.FindStyle(NaxoGUIStyleStyles.GUIStyleType.Toolbar.ToString()));
                        GUILayout.Button(naxoApiHelper.User.Username, GUI.skin.FindStyle(NaxoGUIStyleStyles.GUIStyleType.ProgressBarText.ToString()));
                        if (!hasSDK)
                            if (GUILayout.Button("Install nanoSDK", EditorStyles.toolbarButton))
                                GetWindow(typeof(NANOSDKInstaller));
                        //check if user is in playmode
                        if (Naxoinit.IsPlayMode())
                        {
                            if (GUILayout.Button("Stop PlayMode", EditorStyles.toolbarButton))
                                EditorApplication.isPlaying = false;
                        }
                        else
                        {
                            if (GUILayout.Button("Start PlayMode", EditorStyles.toolbarButton))
                                EditorApplication.isPlaying = true;
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Logout", EditorStyles.toolbarButton))
                        {
                            naxoApiHelper.Logout();
                            GetWindow<naxokitDashboard>().Close();
                            GetWindow<naxokitDashboard>().Show();
                            return;
                        }
                        GUILayout.Button(naxoApiHelper.User.Permission.ToString(), GUI.skin.FindStyle(NaxoGUIStyleStyles.GUIStyleType.ProgressBarText.ToString()));
                        GUILayout.EndHorizontal();

                        EditorGUILayout.BeginVertical();
                        foreach (DictionaryEntry tool in ToolNames())
                        {
                            if (naxoApiHelper.IsUserLoggedIn())
                            {
                                switch (tool.Key.ToString())
                                {
                                    case "Settings":
                                        SettingsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, SettingsOpen);
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
                                        CreditsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, CreditsOpen);
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
                                        if (Config.IsPremiumBoolSinceLastCheck)
                                        {
                                            PremiumOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, PremiumOpen);
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
                                        if (Naxoinit.IsPlayMode())
                                        {
                                            PlayingOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, PlayingOpen);
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
                                            VRCToolsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, VRCToolsOpen);
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
                                        ToolsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, ToolsOpen);
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
                        if (UserIsUptoDate)
                        {
                            EditorGUILayout.LabelField("V" + Config.Version, EditorStyles.centeredGreyMiniLabel);
                            if(GUILayout.Button("Switch Version", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
                                SwitchVersion.ShowWindow();
                        }
                    }
                    #endregion
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
