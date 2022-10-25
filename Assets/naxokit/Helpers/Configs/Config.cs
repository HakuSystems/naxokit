using naxokit.Helpers.Logger;
using naxokit.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace naxokit.Helpers.Configs
{
    public class Config
    {
        //PresetManager
        public static Dictionary<int, PresetData> Presets { get; set; }
         //Default Path
        public static string DefPath {get; set;}
        //NaxoVersion
        public static string Url  { get; set; }
        public static string Version {get; set;}
        
        public static NaxoVersionData.BranchType Branch { get; set; }
        public static  string Commit {get; set;}
        public  static  string CommitUrl {get; set;}
        public static  string CommitDate {get; set;}
        public static bool CheckForUpdates { get; set; }

        //TermsPolicy
        public static bool TermsPolicyAccepted { get; set; }

        //NaxoAuth
        public static string AuthKey { get; set; }
        public static string Password { get; set; }

        //PremiumCheck
        public static DateTime LastPremiumCheck { get; set; }
        public static bool IsPremiumBoolSinceLastCheck { get; set; }

        //DiscordRPC
        public static bool Discordrpc_Enabled { get; set; }
        public static bool Discordrpc_Username { get; set; }

        //SceneSaver
        public static bool SceneAutosaver_Enabled { get; set; }

        //BackupManager
        public static bool BackupManager_SaveAsUnitypackage_Enabled { get; set; }
        public static bool BackupManager_DeleteOldBackups_Enabled { get; set; }
        public static bool BackupManager_AutoBackup_Enabled { get; set; }
        //NaxoPlayMode Tools
        public static bool NaxoPlayModeTools_Enabled { get; set; }

        public static void InitializeConfig()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "naxokit");
            string configPath = Path.Combine(specificFolder, "config.json");
            if (!File.Exists(configPath)) return;
            var config = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(configPath));
            UpdateData(config);
        }
        public static void UpdateConfig()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "naxokit");
            Directory.CreateDirectory(specificFolder);
            string configPath = Path.Combine(specificFolder, "config.json");
            ConfigData config;
            //check if config file exists
            if (!File.Exists(configPath))
            {
                //create config file
                config = new ConfigData();

                WriteDefaults(config);
                UpdateData(config);

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            else
            {
                //Write to config file
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<ConfigData>(json);

                WritetoConfig(config);

                string json2 = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json2);
            }
        }

        private static void UpdateData(ConfigData config) //when Config file Updates the values
        {
            //PresetManager
            Presets = config.PresetManager.Presets;

            //Default Path
            DefPath = config.DefaultPath.DefPath;
            
            //NaxoVersion
            Url = config.NaxoVersion.Url;
            Version = config.NaxoVersion.Version;
            Branch = config.NaxoVersion.Branch;
            Commit = config.NaxoVersion.Commit;
            CommitUrl = config.NaxoVersion.CommitUrl;
            CommitDate = config.NaxoVersion.CommitDate;
            CheckForUpdates = config.NaxoVersion.CheckForUpdates;
            
            
            //TermsPolicy
            TermsPolicyAccepted = config.TermsPolicy.Accepted;

            //NaxoAuth
            AuthKey = config.NaxoAuth.AuthKey;
            Password = config.NaxoAuth.Password;

            //PremiumCheck
            LastPremiumCheck = config.PremiumCheck.LastPremiumCheck;
            IsPremiumBoolSinceLastCheck = config.PremiumCheck.IsPremiumBoolSinceLastCheck;

            //discord
            Discordrpc_Enabled = config.Discord.Enabled;
            Discordrpc_Username = config.Discord.Username;

            //SceneSaver
            SceneAutosaver_Enabled = config.SceneSaver.Enabled;

            //BackupManager
            BackupManager_SaveAsUnitypackage_Enabled = config.BackupManager.SaveAsUnitypackage;
            BackupManager_DeleteOldBackups_Enabled = config.BackupManager.DeleteOldBackups;
            BackupManager_AutoBackup_Enabled = config.BackupManager.AutoBackup;
            //NaxoPlayMode Tools
            NaxoPlayModeTools_Enabled = config.NaxoPlayModeTools.Enabled;
        }

        private static void WritetoConfig(ConfigData config)
        {
            //PresetManager
            config.PresetManager.Presets = Presets;
            
            //Default Path
            config.DefaultPath.DefPath = DefPath;
            
            //NaxoVersion
            config.NaxoVersion.Url = Url;
            config.NaxoVersion.Version = Version;
            config.NaxoVersion.Branch = Branch;
            config.NaxoVersion.Commit = Commit;
            config.NaxoVersion.CommitUrl = CommitUrl;
            config.NaxoVersion.CommitDate = CommitDate;
            config.NaxoVersion.CheckForUpdates = CheckForUpdates;
            
            //TermsPolicy
            config.TermsPolicy.Accepted = TermsPolicyAccepted;


            //NaxoAuth
            config.NaxoAuth.AuthKey = AuthKey;
            config.NaxoAuth.Password = Password;

            //PremiumCheck
            config.PremiumCheck.LastPremiumCheck = LastPremiumCheck;
            config.PremiumCheck.IsPremiumBoolSinceLastCheck = IsPremiumBoolSinceLastCheck;
            //discord
            config.Discord.Enabled = Discordrpc_Enabled;
            config.Discord.Username = Discordrpc_Username;

            //SceneSaver
            config.SceneSaver.Enabled = SceneAutosaver_Enabled;

            //BackupManager
            config.BackupManager.SaveAsUnitypackage = BackupManager_SaveAsUnitypackage_Enabled;
            config.BackupManager.DeleteOldBackups = BackupManager_DeleteOldBackups_Enabled;
            config.BackupManager.AutoBackup = BackupManager_AutoBackup_Enabled;
            //NaxoPlayMode Tools
            config.NaxoPlayModeTools.Enabled = NaxoPlayModeTools_Enabled;
        }
        private static void WriteDefaults(ConfigData config) //When Config file was created
        {
            //PresetManager
            //nothing to do here
            
            //Default Path
            config.DefaultPath.DefPath = null;
            
            //NaxoVersion
            config.NaxoVersion.Url = "";
            var version = File.ReadAllText("Assets/naxokit/Version.txt");
            if (!File.Exists(DefPath + "Version.txt"))
            {
                File.Create(DefPath + "Version.txt");
                File.WriteAllText(DefPath + "Version.txt", version);
            }
            config.NaxoVersion.Version = string.IsNullOrEmpty(File.ReadAllText(DefPath+"Version.txt")) ? version : File.ReadAllText(DefPath+"Version.txt");
            config.NaxoVersion.Branch = NaxoVersionData.BranchType.Release;
            config.NaxoVersion.Commit = "Unknown";
            config.NaxoVersion.CommitUrl = "Unknown";
            config.NaxoVersion.CommitDate = "Unknown";
            config.NaxoVersion.CheckForUpdates = true;
            
            //TermsPolicy
            config.TermsPolicy.Accepted = false;


            //NaxoAuth
            config.NaxoAuth.AuthKey = "";
            config.NaxoAuth.Password = "";


            //PremiumCheck
            config.PremiumCheck.LastPremiumCheck = DateTime.MinValue;
            config.PremiumCheck.IsPremiumBoolSinceLastCheck = false;


            //discord
            config.Discord.Enabled = true;
            config.Discord.Username = true;

            //scene saver
            config.SceneSaver.Enabled = false;

            //BackupManager
            config.BackupManager.SaveAsUnitypackage = false;
            config.BackupManager.DeleteOldBackups = false;
            config.BackupManager.AutoBackup = false;
            //NaxoPlayMode Tools
            config.NaxoPlayModeTools.Enabled = false;
        }
    }
}
