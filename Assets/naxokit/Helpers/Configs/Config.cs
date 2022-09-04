using naxokit.Helpers.Logger;
using naxokit.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace naxokit.Helpers.Configs
{
    public class Config
    {
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
        public static bool BackupManager_SaveinProjectFolder_Enabled { get; set; }
        public static bool BackupManager_DeleteOldBackups_Enabled { get; set; }
        public static string BackupManager_BackupFolder_Selected { get; set; }
        public static bool BackupManager_AutoBackup_Enabled { get; set; }
        //NaxoPlayMode Tools
        public static bool NaxoPlayModeTools_Enabled { get; set; }

        public static void InitializeConfig()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "naxokit");
            string configPath = Path.Combine(specificFolder, "config.json");
            if (File.Exists(configPath))
            {
                var config = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(configPath));
                UpdateData(config);
            }
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
            BackupManager_BackupFolder_Selected = config.BackupManager.BackupFolder;
            BackupManager_DeleteOldBackups_Enabled = config.BackupManager.DeleteOldBackups;
            BackupManager_AutoBackup_Enabled = config.BackupManager.AutoBackup;
            BackupManager_SaveinProjectFolder_Enabled = config.BackupManager.SaveinProjectFolder;
            //NaxoPlayMode Tools
            NaxoPlayModeTools_Enabled = config.NaxoPlayModeTools.Enabled;
        }

        private static void WritetoConfig(ConfigData config)
        {
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
            config.BackupManager.BackupFolder = BackupManager_BackupFolder_Selected;
            config.BackupManager.DeleteOldBackups = BackupManager_DeleteOldBackups_Enabled;
            config.BackupManager.SaveinProjectFolder = BackupManager_SaveinProjectFolder_Enabled;
            config.BackupManager.AutoBackup = BackupManager_AutoBackup_Enabled;
            //NaxoPlayMode Tools
            config.NaxoPlayModeTools.Enabled = NaxoPlayModeTools_Enabled;
        }
        private static void WriteDefaults(ConfigData config) //When Config file was created
        {
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
            config.BackupManager.SaveinProjectFolder = false;
            config.BackupManager.DeleteOldBackups = false;
            config.BackupManager.BackupFolder = "";
            config.BackupManager.AutoBackup = false;
            //NaxoPlayMode Tools
            config.NaxoPlayModeTools.Enabled = false;
        }
    }
}
