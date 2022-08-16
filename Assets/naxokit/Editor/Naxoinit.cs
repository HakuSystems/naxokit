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
            var currentScene = EditorSceneManager.GetActiveScene();

            if (Config.SceneAutosaver_Enabled)
            {
                naxoLog.Log("SceneSaver", "Saving scene: " + currentScene.name);
                EditorSceneManager.SaveScene(currentScene);
            }
        }
    }
}
