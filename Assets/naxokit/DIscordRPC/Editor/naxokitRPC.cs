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
        private static DiscordRpc.RichPresence richPresence = new DiscordRpc.RichPresence();
        
        private static DiscordRpc.EventHandlers handlers = default;

        static naxokitRPC()
        {
            DiscordRpc.Initialize("997507930909855754", ref handlers, false, string.Empty);
            UpdateRPC();
        }

        public static void UpdateRPC()
        {
            if(Config.Discordrpc_Enabled)
            {
                var version = naxokitUpdater.CurrentVersion.Split(';');
                naxoLog.Log("naxokitRPC", "Updating RichPresence");
                if (naxoApiHelper.User != null)
                {
                    //check if username is enabled in config file
                    if (Config.Discordrpc_Username)
                        richPresence.details = "Username: " + naxoApiHelper.User.Username;
                    else
                        richPresence.details = "Username Hidden";
                    //richPresence.details = $"Username: {naxoApiHelper.User.Username}";
                    richPresence.state = "Permission: " + naxoApiHelper.User.Permission.ToString();
                }
                else
                {
                    richPresence.details = "Not logged in"; 
                    richPresence.state = "";
                }
                
                richPresence.largeImageKey = "big";
                richPresence.largeImageText = "In Unity with naxokit";
                richPresence.smallImageKey = "edit";
                richPresence.smallImageText = "ver " + version[0];
                DiscordRpc.UpdatePresence(richPresence);
            }
            
        }
    }
}

