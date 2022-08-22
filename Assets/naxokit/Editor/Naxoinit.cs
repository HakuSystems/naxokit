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
        }
        private static void SceneSaver()
        {
            EditorApplication.hierarchyChanged += () =>
            {
                if (Config.SceneAutosaver_Enabled && !Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) //only works in Edit Mode
                {
                    EditorSceneManager.SaveOpenScenes();
                    naxoLog.Log("SceneSaver", "Open Scenes saved");
                }
            };
        }
    }
}
