using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Updater;
using UnityEditor;

namespace naxokit.DiscordRPC
{
    [InitializeOnLoad]
    public class naxokitRPC
    {
        private static DiscordRpc.RichPresence richPresence = new DiscordRpc.RichPresence();
        private static DiscordRpc.EventHandlers handlers = default;

        static naxokitRPC()
        {
            DiscordRpc.Initialize("997507930909855754", ref handlers, false, string.Empty);
            UpdateRPC();
        }

        public static void UpdateRPC()
        {
            // TODO add actual username
            var version = naxokitUpdater.CurrentVersion.Split(';');
            naxoLog.Log("naxokitRPC", "Updating RichPresence");
            if (naxokit.Screens.Auth.naxoApiHelper.IsLoggedInAndVerified())
                richPresence.state = $"Username: {naxokit.Screens.Auth.naxoApiHelper.User.Username}";
            else
                richPresence.state = "Not logged in";
            richPresence.details = "naxokit.com";
            richPresence.largeImageKey = "big";
            richPresence.largeImageText = "In Unity with naxokit";
            richPresence.smallImageKey = "edit";
            richPresence.smallImageText = "ver " + version[0];
            DiscordRpc.UpdatePresence(richPresence);
        }
    }
}

