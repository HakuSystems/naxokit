using UnityEngine;

namespace naxokit.Helpers.Logger
{
    public class naxoLog
    {
        public static void Log(string title, string message)
        {
            Debug.Log($"[{title}]: " + "<color=green>" + message + "</color>");
        }
        public static void LogWarning(string title, string message)
        {
            Debug.LogWarning($"[{title}]: " + "<color=yellow>" + message + "</color>");
        }
    }
}
