using UnityEditor;
using naxokit.Helpers.Configs;
using UnityEditor.SceneManagement;
using naxokit.Helpers.Logger;
using UnityEngine.SceneManagement;

namespace naxokit
{
    [InitializeOnLoad]
    class Naxoinit
    {
        static Naxoinit()
        {
            Config.InitializeConfig();
            SceneSaver();
            if (Config.BackupManager_AutoBackup_Enabled)
                BackupManager.CreateBackup(Config.BackupManager_SaveAsUnitypackage_Enabled,
                    Config.BackupManager_DeleteOldBackups_Enabled);
        }
        public static bool IsPlayMode()
        {
            var scenePath = "Assets/naxokit/Helpers/Scenes/naxokitPlayModeTools.unity";
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) return false;
            if (!Config.NaxoPlayModeTools_Enabled) return true;
            if (!SceneManager.GetSceneByPath(scenePath).isLoaded)
                EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Additive));

            return true;
        }

        private static void SceneSaver()
        {
            EditorApplication.hierarchyChanged += () =>
            {
                if (!Config.SceneAutosaver_Enabled || IsPlayMode()) return; //only works in Edit Mode
                EditorSceneManager.SaveOpenScenes();
                naxoLog.Log("SceneSaver", "Open Scenes saved");
            };
        }
    }
}
