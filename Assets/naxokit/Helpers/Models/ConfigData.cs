namespace naxokit.Helpers.Models
{
    public class ConfigData
    {
        public ConfigData()
        {
            Discord = new DiscordData();
            SceneSaver = new SceneSaverData();
        }
        public SceneSaverData SceneSaver { get; set; }
        public DiscordData Discord { get; set; }
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
