using System;
using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Styles;
using naxokit.Updater;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using naxokit.Screens;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace naxokit
{
    public class naxokitDashboard : EditorWindow
    {
        private bool _settingsOpen;
        private bool _creditsOpen;
        [FormerlySerializedAs("ToolsOpen")] public bool toolsOpen;
        private bool _premiumOpen;
        private static bool _loginOpen = true;
        private static bool _signUpOpen;
        private static bool _playingOpen;
        private Vector2 _scrollPosition;
        public static bool userIsUptoDate = false;
        public static bool finallyLoggedIn;
        public static bool savePasswordLocally;
        private static string _passStatus = "";
        private static string _usernameInput;
        private static string _passwordInput;
        private static string _emailInput;
        private static string _redeemCode;
        private Task _task;


        [MenuItem("naxokit/Dashboard")]
        public static void ShowWindow() => GetWindow(typeof(naxokitDashboard));
        

        private void OnLostFocus()
        {
            Settings.UpdateConfigsAndChangeRPC();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Dashboard");
            minSize = new Vector2(1000, 300);
            Focus();
            _task = naxokitUpdater.CheckForUpdates();
            
            if (Config.DefPath == null)
            {
                NaxoDefaultPath.ShowWindow();
                Close();
            }
            
        }

        private void Update()
        {
            _loginOpen = true; //Prefending the user from closing the Foldout while logging in.
            if (Config.DefPath != null) return;
            NaxoDefaultPath.ShowWindow();
            Close();
        }
        public static void SetFinallyLoggedIn(bool isLoggedIn) => finallyLoggedIn = naxoApiHelper.IsLoggedInAndVerified() && isLoggedIn;
        
        private static Hashtable ToolNames()
        {
            var toolsName = new Hashtable(){
                {"Tools", Resources.Load("Tools")as Texture2D},
                {"Settings", Resources.Load("Settings") as Texture2D},
                {"Premium", Resources.Load("Premium") as Texture2D},
                {"Credits", Resources.Load("Credits") as Texture2D}
            };
            return toolsName;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false, GUILayout.Width(EditorGUIUtility.currentViewWidth));
                {
                    #region Login and Signup
                    EditorGUILayout.BeginVertical();
                    {
                        if (!naxoApiHelper.IsUserLoggedIn())
                        {
                            _loginOpen = FoldoutTexture.MakeTextureFoldout(Resources.Load("Login") as Texture2D, _loginOpen);
                            if (_loginOpen)
                            {
                                DrawLine.DrawHorizontalLine();
                                EditorGUILayout.LabelField("Login");
                                _usernameInput = EditorGUILayout.TextField("Username", _usernameInput);
                                _passwordInput = EditorGUILayout.PasswordField("Password", _passwordInput);
                                var termsOfService = new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.ScriptText.ToString());
                                EditorGUILayout.BeginHorizontal();
                                {
                                    savePasswordLocally = EditorGUILayout.Toggle("Save Password Locally", savePasswordLocally);
                                    EditorGUILayout.LabelField(_passStatus, EditorStyles.centeredGreyMiniLabel);
                                    _passStatus = "no Password Saved";
                                    if (Config.Password != "")
                                    {
                                        _passStatus = "Password available";
                                        savePasswordLocally = true;
                                        _passwordInput = naxoApiHelper.GetSavedPassword();
                                        if (GUILayout.Button("Clear Password"))
                                        {
                                            Config.Password = null;
                                            _passwordInput = null;
                                            _usernameInput = null;
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
                                    if (string.IsNullOrEmpty(_usernameInput) || string.IsNullOrEmpty(_passwordInput))
                                        EditorUtility.DisplayDialog("Login", "Credentials cant be Empty", "Okay");
                                    else
                                        naxoApiHelper.Login(_usernameInput, _passwordInput);
                                }
                            }
                            _signUpOpen = FoldoutTexture.MakeTextureFoldout(Resources.Load("SignUp") as Texture2D, _signUpOpen);
                            if (_signUpOpen)
                            {
                                DrawLine.DrawHorizontalLine();
                                _usernameInput = EditorGUILayout.TextField("Username", _usernameInput);
                                _passwordInput = EditorGUILayout.PasswordField("Password", _passwordInput);
                                _emailInput = EditorGUILayout.TextField("Email", _emailInput);
                                EditorGUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button("Generate Strong Password"))
                                    {
                                        _passwordInput = naxoApiHelper.ApiGenerateStrongPassword();
                                    }
                                    if (GUILayout.Button("Create Account"))
                                    {
                                        if (string.IsNullOrEmpty(_usernameInput) || string.IsNullOrEmpty(_passwordInput) || string.IsNullOrEmpty(_emailInput))
                                            EditorUtility.DisplayDialog("SignUp", "Credentials cant be Empty", "Okay");
                                        else
                                            naxoApiHelper.SignUp(_usernameInput, _passwordInput, _emailInput);
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
                                _redeemCode = EditorGUILayout.TextField("License Key", _redeemCode);
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
                                if (string.IsNullOrEmpty(_redeemCode))
                                    EditorUtility.DisplayDialog("naxokitDashboard", "License Key cant be Empty", "Okay");
                                else
                                    naxoApiHelper.RedeemLicense(_redeemCode);
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
                        GUILayout.Button(naxoApiHelper.user.Username, GUI.skin.FindStyle(NaxoGUIStyleStyles.GUIStyleType.ProgressBarText.ToString()));
                        if(GUILayout.Button("Switch Version", new GUIStyle(NaxoGUIStyleStyles.GUIStyleType.toolbarbutton.ToString())))
                            SwitchVersion.ShowWindow();
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
                        GUILayout.Button(naxoApiHelper.user.Permission.ToString(), GUI.skin.FindStyle(NaxoGUIStyleStyles.GUIStyleType.ProgressBarText.ToString()));
                        GUILayout.EndHorizontal();

                        EditorGUILayout.BeginVertical();
                        foreach (var tool in ToolNames().Cast<DictionaryEntry>().Where(tool => naxoApiHelper.IsUserLoggedIn()))
                        {
                            switch (tool.Key.ToString())
                            {
                                
                                case "Credits":
                                    CreditsFoldoutOpen(tool);
                                    break;
                                case "Settings":
                                    SettingsFoldoutOpen(tool);
                                    break;
                                case "Premium":
                                    PremiumFoldoutOpen(tool);
                                    break;
                                case "Tools":
                                    ToolsFoldoutOpen(tool);
                                    break;
                            }
                        }
                        if (userIsUptoDate)
                        {
                                DrawLine.DrawHorizontalLine();
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("V" + Config.Version, EditorStyles.largeLabel);
                            }
                            EditorGUILayout.EndHorizontal();
                            
                        }
                    }
                    #endregion
                }
                EditorGUILayout.EndScrollView();
                
            }
        }

        private void ToolsFoldoutOpen(DictionaryEntry tool)
        {
            toolsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, toolsOpen);
            if (!toolsOpen) return;
            EditorGUILayout.BeginVertical();
            {
                naxokit.Screens.naxoTools.HandleToolsOpend();
            }
            EditorGUILayout.EndVertical();
        }

        private void PremiumFoldoutOpen(DictionaryEntry tool)
        {
            if (!Config.IsPremiumBoolSinceLastCheck) return;
            _premiumOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, _premiumOpen);
            if (!_premiumOpen) return;
            EditorGUILayout.BeginVertical();
            {
                naxokit.Screens.Premium.HandlePremiumOpend();
            }
            EditorGUILayout.EndVertical();
        }

        private void SettingsFoldoutOpen(DictionaryEntry tool)
        {
            _settingsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, _settingsOpen);
            if (!_settingsOpen) return;
            EditorGUILayout.BeginVertical();
            {
                Settings.HandleSettingsOpend();
            }
            EditorGUILayout.EndVertical();
        }

        private void CreditsFoldoutOpen(DictionaryEntry tool)
        {
            _creditsOpen = FoldoutTexture.MakeTextureFoldout((Texture2D)tool.Value, _creditsOpen);
            if (!_creditsOpen) return;
            EditorGUILayout.BeginVertical();
            {
                Credits.HandleCreditsOpend();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
