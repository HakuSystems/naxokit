using System;
using System.Security;
using JetBrains.Annotations;

namespace naxokit.Helpers.Models
{
    public class ConfigData
    {
        public ConfigData()
        {
            NaxoAuth = new NaxoAuthData();
            Discord = new DiscordData();
            SceneSaver = new SceneSaverData();
            BackupManager = new BackupManagerData();
            PremiumCheck = new PremiumCheckData();
        }
        public NaxoAuthData NaxoAuth { get; set; }
        public PremiumCheckData PremiumCheck { get; set; }
        public SceneSaverData SceneSaver { get; set; }
        public DiscordData Discord { get; set; }
        public BackupManagerData BackupManager { get; set; }
    }

    public class NaxoAuthData
    {
        [CanBeNull] public string AuthKey { get; set; }
        [CanBeNull] public string Password { get; set; }
    }

    public class PremiumCheckData
    {
        public DateTime LastPremiumCheck { get; set; }
        public bool IsPremiumBoolSinceLastCheck { get; set; }
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
