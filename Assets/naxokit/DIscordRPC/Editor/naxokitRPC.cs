using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Updater;
using UnityEditor;

namespace naxokit.DISCORDRPC
{
    [InitializeOnLoad]
    public class naxokitRPC
    {
        private static DiscordRpc.RichPresence richPresence = new DiscordRpc.RichPresence();
        private static DiscordRpc.EventHandlers handlers = default;

        static naxokitRPC()
        {
            if (discord_rich.GetCurrentDiscordBool())
            {
                DiscordRpc.Initialize("997507930909855754", ref handlers, false, string.Empty);
                UpdateRPC();
            }
        }

        private static void UpdateRPC()
        {
            var version = naxokitUpdater.CurrentVersion.Split(';');
            naxoLog.Log("naxokitRPC", "Updating RichPresence");
            richPresence.state = "Username: lyze";//Hardcored temp
            richPresence.details = "naxokit.com";
            richPresence.largeImageKey = "big";
            richPresence.largeImageText = "In Unity with naxokit";
            richPresence.smallImageKey = "edit";
            richPresence.smallImageText = "ver " + version[0];
            DiscordRpc.UpdatePresence(richPresence);
        }

        //todo add actuall account acessability
        public static void ChangeStateRPC(string dataString = "Username: Hidden")
        {
            richPresence.state = dataString;
            DiscordRpc.UpdatePresence(richPresence);
        }
    }
}

