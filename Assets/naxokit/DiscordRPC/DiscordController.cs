using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using naxokit.Updater;
using Discord;

[InitializeOnLoad]
public class DiscordController
{
    public static Discord.Discord discord;

    static DiscordController()
    {
        Debug.Log("DiscordController");
        discord = new Discord.Discord(997507930909855754, (System.UInt64)Discord.CreateFlags.Default);
        var acitivityManager = discord.GetActivityManager();
        var version = naxokitUpdater.CurrentVersion.Split(';');
        var acitivity = new Discord.Activity() {
            State = naxokitUpdater.CurrentVersion.Replace(";", " "),
            Details = "naxokit.com",
            Assets = new Discord.ActivityAssets() {
                LargeImage = "big",
                LargeText = "naxokit",
                SmallImage = "small",
                SmallText =  "ver " + version[0]
            },
        };
        acitivityManager.UpdateActivity(acitivity, (Discord.Result result) => {
            if (result == Discord.Result.Ok)
            {
                Debug.Log("Activity updated");
            }
            else
            {
                Debug.Log("Activity update failed");
            }
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

