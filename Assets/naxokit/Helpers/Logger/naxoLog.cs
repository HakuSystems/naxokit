﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace naxokit.Helpers.Logger
{
    public class naxoLog
    {
        public static void Log(string title, string message)
        {
            
            Debug.Log($"[{title}]: "+ "<color=green>"+message+"</color>");
        }
        public static void LogError(string title, string message)
        {
            Debug.LogError($"[{title}]: " + "<color=red>" + message + "</color>");
        }
        public static void LogWarning(string title, string message)
        {
            Debug.LogWarning($"[{title}]: " + "<color=yellow>" + message + "</color>");
        }
    }
}
