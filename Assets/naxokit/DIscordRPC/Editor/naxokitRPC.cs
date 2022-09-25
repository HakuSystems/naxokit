using naxokit.Helpers.Auth;
using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Updater;
using UnityEditor;

namespace naxokit.DiscordRPC
{
    [InitializeOnLoad]
    public class naxokitRPC
    {
        private static readonly DiscordRpc.RichPresence RichPresence = new DiscordRpc.RichPresence();
        
        private static readonly DiscordRpc.EventHandlers Handlers = default;

        static naxokitRPC()
        {
            DiscordRpc.Initialize("997507930909855754", ref Handlers, false, string.Empty);
            UpdateRPC();
        }

        public static void UpdateRPC()
        {
            if (!Config.Discordrpc_Enabled) return;
            naxoLog.Log("naxokitRPC", "Updating RichPresence");
            if (naxoApiHelper.IsLoggedInAndVerified())
            {
                //check if username is enabled in config file
                if (Config.Discordrpc_Username)
                    RichPresence.details = "Username: " + naxoApiHelper.user.Username;
                else
                    RichPresence.details = "Username Hidden"; 
                //richPresence.details = $"Username: {naxoApiHelper.User.Username}";
                RichPresence.state = "Permission: " + naxoApiHelper.user.Permission.ToString();
            }
            else
            {
                RichPresence.details = "Not logged in"; 
                RichPresence.state = "";
            }
                
            RichPresence.largeImageKey = "big";
            RichPresence.largeImageText = "In Unity with naxokit";
            RichPresence.smallImageKey = "edit";
            RichPresence.smallImageText = "ver " + Config.Version;
            DiscordRpc.UpdatePresence(RichPresence);

        }
    }
}

