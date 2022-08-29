using System.Net.Mime;
using JetBrains.Annotations;
using naxokit.Helpers.Logger;
using naxokit.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace naxokit.Helpers.Configs
{
    public class Config
    {
        //BackupManager
        private static bool backupManager_saveAsUnitypackage_Enabled = true;
        private static bool backupManager_saveinProjectFolder_Enabled = true;
        private static bool backupManager_deleteOldBackups_Enabled = false;
        private static bool backupManager_autoBackup_Enabled = false;
        private static string backupManager_backupFolder_Selected = "";

        public static bool BackupManager_SaveAsUnitypackage_Enabled
        {
            get { return backupManager_saveAsUnitypackage_Enabled; }
            set { backupManager_saveAsUnitypackage_Enabled = value; }
        }
        public static bool BackupManager_SaveinProjectFolder_Enabled
        {
            get { return backupManager_saveinProjectFolder_Enabled; }
            set { backupManager_saveinProjectFolder_Enabled = value; }
        }
        public static bool BackupManager_DeleteOldBackups_Enabled
        {
            get { return backupManager_deleteOldBackups_Enabled; }
            set { backupManager_deleteOldBackups_Enabled = value; }
        }
        public static string BackupManager_BackupFolder_Selected
        {
            get { return backupManager_backupFolder_Selected; }
            set { backupManager_backupFolder_Selected = value; }
        }
        public static bool BackupManager_AutoBackup_Enabled
        {
            get { return backupManager_autoBackup_Enabled; }
            set { backupManager_autoBackup_Enabled = value; }
        }

        //SceneSaver
        private static bool sceneAutosaver_Enabled = false;
        public static bool SceneAutosaver_Enabled
        {
            get { return sceneAutosaver_Enabled; }
            set { sceneAutosaver_Enabled = value; }
        }

        //DiscordRPC
        private static bool discordrpc_Enabled = false;
        private static bool discordrpc_Username = false;
        public static bool Discordrpc_Enabled
        {
            get { return discordrpc_Enabled; }
            set { discordrpc_Enabled = value; }
        }
        public static bool Discordrpc_Username
        {
            get { return discordrpc_Username; }
            set { discordrpc_Username = value; }
        }

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
            //discord
            discordrpc_Enabled = config.Discord.Enabled;
            discordrpc_Username = config.Discord.Username;
            //BackupManager
            BackupManager_SaveAsUnitypackage_Enabled = config.BackupManager.SaveAsUnitypackage;
            backupManager_backupFolder_Selected = config.BackupManager.BackupFolder;
            BackupManager_DeleteOldBackups_Enabled = config.BackupManager.DeleteOldBackups;
            BackupManager_AutoBackup_Enabled = config.BackupManager.AutoBackup;
            BackupManager_SaveinProjectFolder_Enabled = config.BackupManager.SaveinProjectFolder;
        }

        private static void WritetoConfig(ConfigData config)
        {
            config.Discord.Enabled = discordrpc_Enabled;
            config.Discord.Username = discordrpc_Username;

            config.BackupManager.SaveAsUnitypackage = BackupManager_SaveAsUnitypackage_Enabled;
            config.BackupManager.BackupFolder = BackupManager_BackupFolder_Selected;
            config.BackupManager.DeleteOldBackups = BackupManager_DeleteOldBackups_Enabled;
            config.BackupManager.SaveinProjectFolder = BackupManager_SaveinProjectFolder_Enabled;
            config.BackupManager.AutoBackup = BackupManager_AutoBackup_Enabled;
        }
        private static void WriteDefaults(ConfigData config) //When Config file was created
        {
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
        }

        private static void Debug_DumpValues() 
        {
            naxoLog.Log("Debug", Convert.ToString(discordrpc_Enabled));
            naxoLog.Log("Debug", Convert.ToString(discordrpc_Username));
        }
    }
}
