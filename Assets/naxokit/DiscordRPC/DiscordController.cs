using System.Threading;
using UnityEngine;
using UnityEditor;
using naxokit.Updater;
using System;
using System.IO;
using naxokit.Helpers.Configs;
//Bug Somehow it created more instances of discords rich presence and doesnt overwrite the old one nor then clears the old one
//and creates a new one
[InitializeOnLoad]
public class DiscordController
{
    public static Discord.Discord discord;
    private static TimeSpan time = (DateTime.UtcNow - new DateTime(1970, 1, 1));
    private static long timestamp = (long)time.TotalSeconds;
    static DiscordController()
    {
        if (discord_rich.GetCurrentDiscordBool())
        {
            discord = new Discord.Discord(997507930909855754, (System.UInt64)Discord.CreateFlags.Default);
            var acitivityManager = discord.GetActivityManager();
            var version = naxokitUpdater.CurrentVersion.Split(';');
            var acitivity = new Discord.Activity()
            {
                State = "Username: lyze", //Temp hardcoded
                Details = "naxokit.com",
                Assets = new Discord.ActivityAssets()
                {
                    LargeImage = "big",
                    LargeText = "in Unity with naxokit",
                    SmallImage = "small",
                    SmallText = "ver " + version[0]
                },
                Timestamps = new Discord.ActivityTimestamps() { Start = timestamp }

            };
            acitivityManager.UpdateActivity(acitivity, (Discord.Result result) => {
                if (result == Discord.Result.Ok)
                    Debug.Log("Discord Activity updated");
                else
                    Debug.Log("Discord Activity update failed");
            });
            var callback = new Thread(() =>
            {
                while (true)
                {
                    discord.RunCallbacks();
                    Thread.Sleep(10);
                }
            });
            callback.Start();
        }
    }
}