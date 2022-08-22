using UnityEditor;
using UnityEngine;
using naxokit.Helpers.Configs;
using System;
using System.Threading;
using UnityEditor.SceneManagement;
using naxokit.Helpers.Logger;

namespace naxokit
{
    [InitializeOnLoad]
    class Naxoinit
    {
        static Naxoinit()
        {
            Config.InitializeConfig();
            SceneSaver();
            IsPlayMode();
        }

        public static bool IsPlayMode()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
                return true;
            else
                return false;
        }

        private static void SceneSaver()
        {
            EditorApplication.hierarchyChanged += () =>
            {
                if (Config.SceneAutosaver_Enabled && !IsPlayMode()) //only works in Edit Mode
                {
                    EditorSceneManager.SaveOpenScenes();
                    naxoLog.Log("SceneSaver", "Open Scenes saved");
                }
            };
        }
    }
}
