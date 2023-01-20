using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using JetBrains.Annotations;

namespace naxokit.Helpers.Models
{
    public class ConfigData
    {
        public ConfigData()
        {
            NaxoAuth = new NaxoAuthData();
            TermsPolicy = new TermsPolicyData();
            Discord = new DiscordData();
            SceneSaver = new SceneSaverData();
            BackupManager = new BackupManagerData();
            PremiumCheck = new PremiumCheckData();
            NaxoPlayModeTools = new NaxoPlayModeToolsData();
            NaxoVersion = new NaxoVersionData();
            DefaultPath = new DefaultPathData();
            PresetManager = new PresetManagerData();
        }

        public  PresetManagerData PresetManager { get; set; }
        public DefaultPathData DefaultPath { get; set; }

        public NaxoVersionData NaxoVersion { get; set; }
        public NaxoAuthData NaxoAuth { get; set; }
        public TermsPolicyData TermsPolicy { get; set; }
        public NaxoPlayModeToolsData NaxoPlayModeTools { get; set; }
        public PremiumCheckData PremiumCheck { get; set; }
        public SceneSaverData SceneSaver { get; set; }
        public DiscordData Discord { get; set; }
        public BackupManagerData BackupManager { get; set; }
    }

    public class PresetManagerData
    {
        [CanBeNull] public Dictionary<int, PresetData> Presets { get; set; }
    }

    public class PresetData
    {
        public string Name { get; set; }
        public List<string> Paths { get; set; }
        public string Description { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
    }

    public class DefaultPathData
    {
        public string  DefPath { get; set; }
    }

    public class NaxoVersionData
    {
        public string Url { get; set; }
        public string Version { get; set; }
        public BranchType Branch { get; set; }
        public string Commit { get; set; }
        public string CommitUrl { get; set; }
        public string CreatedOn { get; set; }
        public  bool CheckForUpdates { get; set; }
        public enum BranchType
        {
            Release = 0,
            Beta = 1
        }
    }
    public class TermsPolicyData
    {
        public bool Accepted { get; set; }
    }

    public class NaxoPlayModeToolsData
    {
        public bool Enabled { get; set; }
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
        public bool DeleteOldBackups { get; set; }
        public bool AutoBackup { get; set; }
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
