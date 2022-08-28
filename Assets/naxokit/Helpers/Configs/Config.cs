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
        private static bool saveAsUnitypackage_Enabled = true;
        private static bool saveinProjectFolder_Enabled = true;
        private static bool deleteOldBackups_Enabled = false;
        private static bool autoBackup_Enabled = false;
        private static string backupFolder_Selected = "";

        public static bool SaveAsUnitypackage_Enabled
        {
            get { return saveAsUnitypackage_Enabled; }
            set { saveAsUnitypackage_Enabled = value; UpdateConfig(); }
        }
        public static bool SaveinProjectFolder_Enabled
        {
            get { return saveinProjectFolder_Enabled; }
            set { saveinProjectFolder_Enabled = value; UpdateConfig(); }
        }
        public static bool DeleteOldBackups_Enabled
        {
            get { return deleteOldBackups_Enabled; }
            set { deleteOldBackups_Enabled = value; UpdateConfig(); }
        }
        public static string BackupFolder_Selected
        {
            get { return backupFolder_Selected; }
            set { backupFolder_Selected = value; UpdateConfig(); }
        }
        public static bool AutoBackup_Enabled
        {
            get { return autoBackup_Enabled; }
            set { autoBackup_Enabled = value; UpdateConfig(); }
        }

        //SceneSaver
        private static bool sceneAutosaver_Enabled = false;
        public static bool SceneAutosaver_Enabled
        {
            get { return sceneAutosaver_Enabled; }
            set { sceneAutosaver_Enabled = value; UpdateConfig(); }
        }

        //DiscordRPC
        private static bool discordrpc_Enabled = false;
        private static bool discordrpc_Username = false;
        public static bool Discordrpc_Enabled
        {
            get { return discordrpc_Enabled; }
            set { discordrpc_Enabled = value; UpdateConfig(); }
        }
        public static bool Discordrpc_Username
        {
            get { return discordrpc_Username; }
            set { discordrpc_Username = value; UpdateConfig(); }
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

                WriteData(config);

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            else
            {
                //read config file
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<ConfigData>(json);

                UpdateData(config);

                string json2 = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json2);
            }
        }

        private static void UpdateData(ConfigData config) //when Config file Updates the values
        {
            //discord
            config.Discord.Enabled = discordrpc_Enabled;
            config.Discord.Username = discordrpc_Username;

            //scene saver
            config.SceneSaver.Enabled = sceneAutosaver_Enabled;

            //BackupManager
            config.BackupManager.SaveAsUnitypackage = saveAsUnitypackage_Enabled;
            config.BackupManager.SaveinProjectFolder = saveinProjectFolder_Enabled;
            config.BackupManager.DeleteOldBackups = deleteOldBackups_Enabled;
            config.BackupManager.BackupFolder = backupFolder_Selected;
            config.BackupManager.AutoBackup = autoBackup_Enabled;
        }

        private static void WriteData(ConfigData config) //When Config file was created
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
    }
}
