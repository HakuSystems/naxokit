namespace naxokit.Helpers.Models
{
    public class ConfigData
    {
        public ConfigData()
        {
            Discord = new DiscordData();
        }
        public DiscordData Discord { get; set; }
    }
    public class DiscordData
    {
        public bool Enabled { get; set; }
        public bool Username { get; set; }
    }
}
