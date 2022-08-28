using System.Security;
namespace naxokit.Helpers.Models
{
    public class ConfigData
    {
        public ConfigData()
        {
            Discord = new DiscordData();
            SceneSaver = new SceneSaverData();
            BackupManager = new BackupManagerData();
        }
        public SceneSaverData SceneSaver { get; set; }
        public DiscordData Discord { get; set; }
        public BackupManagerData BackupManager { get; set; }
    }

    public class BackupManagerData
    {
        public bool SaveAsUnitypackage { get; set; }
        public bool SaveinProjectFolder { get; set; }
        public bool DeleteOldBackups { get; set; }
        public bool AutoBackup { get; set; }
        public string BackupFolder { get; set; }
    }

    public class SceneSaverData
    {
        public bool Enabled { get; set; }
    }

    public class DiscordData
    {
        public bool Enabled { get; set; }
        public bool Username { get; set; }
    }
}
