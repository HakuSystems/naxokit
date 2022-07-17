using naxokit.Helpers;
using naxokit.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace naxokit.Helpers.Configs
{
    public class discord_rich
    {
        private static bool discord_richBool;

        public static void UpdateDiscordRichConfig(bool toggleState)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "naxokit");
            Directory.CreateDirectory(specificFolder);
            string configPath = Path.Combine(specificFolder, "rich_discord.json");
            if (File.Exists(configPath))
            {
                var config = JsonConvert.DeserializeObject<DiscordData>(File.ReadAllText(configPath));
                config.Enabled = toggleState;
                PlayerPrefs.SetInt("rich_discord", Converters.boolToInt(config.Enabled));
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                discord_richBool = config.Enabled;
            }
            else
            {
                var config = new DiscordData();
                config.Enabled = toggleState;
                PlayerPrefs.SetInt("rich_discord", Converters.boolToInt(config.Enabled));
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                discord_richBool = config.Enabled;
            }
            Debug.Log("Updated Discord Settings");
        }
        public static bool GetCurrentDiscordBool() { return Converters.intToBool(PlayerPrefs.GetInt("rich_discord")); }
    }
}
