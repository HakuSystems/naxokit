using naxokit.Helpers.Logger;
using naxokit.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using UnityEditor.SceneManagement;

namespace naxokit.Helpers.Configs
{
    public class Config
    {
        private static bool sceneAutosaver_Enabled = false;
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

        public static bool SceneAutosaver_Enabled
        {
            get { return sceneAutosaver_Enabled; }
            set { sceneAutosaver_Enabled = value; UpdateConfig(); }
        }

        public static void InitializeConfig()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "naxokit");
            string configPath = Path.Combine(specificFolder, "config.json");
            if (File.Exists(configPath))
            {
                var config = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(configPath));
                //discord
                discordrpc_Enabled = config.Discord.Enabled;
                discordrpc_Username = config.Discord.Username;

                //scene saver
                sceneAutosaver_Enabled = config.SceneSaver.Enabled;
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
                //discord
                config.Discord.Enabled = true;
                config.Discord.Username = true;

                //scene saver
                config.SceneSaver.Enabled = false;

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            else
            {
                //read config file
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<ConfigData>(json);
                //update config file
                //discord
                config.Discord.Enabled = discordrpc_Enabled;
                config.Discord.Username = discordrpc_Username;

                //scene saver
                config.SceneSaver.Enabled = sceneAutosaver_Enabled;

                string json2 = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json2);
            }
        }
    }
}
