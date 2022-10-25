using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Helpers.Models;

namespace naxokit.Updater
{
    public class naxokitUpdater : MonoBehaviour
    {
        private const string ScriptName = "naxokitUpdater";

        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly WebClient WebClient = new WebClient();

        private const string BaseURL = "https://api.naxokit.com";

        private static readonly Uri ReleaseVersionUri = new Uri(BaseURL + "/public/naxokit/version"); //Only for Branch: release
        private static readonly Uri VersionListUri = new Uri(BaseURL + "/public/naxokit/version/list"); //Lists all Branches
        
        private  static NaxoVersionData LatestVersion { get; set; }
        
        private List<NaxoVersionData> _versionList;
        
        public static async Task CheckForUpdates(Enum branch = null)
        {
            
            naxoLog.Log(ScriptName,"Checking for updates...");
            if(branch == null)
                branch = Config.Branch;
            LatestVersion = await GetLatestVersion(branch);
            if (LatestVersion == null)
            {
                naxoLog.Log(ScriptName,"No updates found.");
                naxokitDashboard.userIsUptoDate = true;
                return;
            }
            if (Config.Version != LatestVersion.Version)
            {
                naxoLog.Log(ScriptName,"Update available!");
                if (!Config.CheckForUpdates)
                    return;
                var update = EditorUtility.DisplayDialog(ScriptName,
                    "Version V"+LatestVersion.Version+" is available. Do you want to update?", "Yes", "No");
                if (update)
                {
                    BackupManager.CreateBackup(Config.BackupManager_SaveAsUnitypackage_Enabled,
                        Config.BackupManager_DeleteOldBackups_Enabled);
                    await  DownloadVersion();
                }
                naxokitDashboard.userIsUptoDate = false;
                return;
            }
            naxoLog.Log(ScriptName,"You are up to date!");
            naxokitDashboard.userIsUptoDate = true;
        }

        private static async Task<NaxoVersionData> GetLatestVersion(Enum branch)
        {
            if(branch.Equals(NaxoVersionData.BranchType.Release))
                return await GetReleaseVersion();
            return await GetBetaVersion();
        }
        public static void GetVersionList()
        {
            var response = HttpClient.GetAsync(VersionListUri).Result;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                naxoLog.Log(ScriptName,"Error while getting version list.");
                return;
            }
            var content = response.Content.ReadAsStringAsync().Result;
            var versionList = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<List<NaxoVersionData>>>(content);
            if (versionList != null) SwitchVersion.VersionDataList = versionList.Data;
        }
        private static async Task<NaxoVersionData> GetBetaVersion()
        {
            naxoLog.Log(ScriptName,"Getting latest beta version...");
            var response = await HttpClient.GetAsync(VersionListUri);
            var content = await response.Content.ReadAsStringAsync();
            var versionList = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<List<NaxoVersionData>>>(content);
            System.Diagnostics.Debug.Assert(versionList != null, nameof(versionList) + " != null");
            var latestVersion = versionList.Data.OrderByDescending(x => x.Version).FirstOrDefault(x=> x.Branch.Equals(NaxoVersionData.BranchType.Beta));
            return latestVersion;
        }

        private static async Task<NaxoVersionData> GetReleaseVersion()
        {
            naxoLog.Log(ScriptName,"Getting latest release version...");
            var response = HttpClient.GetAsync(ReleaseVersionUri);
            var content = await response.Result.Content.ReadAsStringAsync();
            var versionResponse = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<NaxoVersionData>>(content);
            return versionResponse?.Data;
        }

        public static Task DownloadVersion(string url = null, string version = null, Enum branch = null)
        {
            try
            {
                WebClient.DownloadFileAsync(new Uri((LatestVersion?.Url ?? url) ?? string.Empty),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    $"/naxokit V{LatestVersion?.Version ?? version}.unitypackage");
                WebClient.DownloadProgressChanged += (sender, args) =>
                {
                    EditorUtility.DisplayProgressBar("Downloading",
                        $"Downloading naxokit V{LatestVersion?.Version ?? version}", args.ProgressPercentage);
                };
                WebClient.DownloadFileCompleted += (sender, args) =>
                {
                    var files = Directory.GetFiles(Application.dataPath + "/naxokit", "*.*", SearchOption.AllDirectories)
                        .Where(s => !s.EndsWith("naxokitUpdater.cs"));


                    EditorUtility.ClearProgressBar();
                    var alleles = files as string[] ?? files.ToArray();
                    foreach (var file in alleles)
                    {
                        if (file.EndsWith(".dll") || file.EndsWith(".meta") || file.EndsWith(".tmp"))
                            continue;
                        File.Delete(file);
                    }
                    AssetDatabase.Refresh();
                    
                    AssetDatabase.ImportPackage(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+$"/naxokit V{LatestVersion?.Version}.unitypackage", false);
                    
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                $"/naxokit V{LatestVersion?.Version ?? version}.unitypackage");
                    EditorUtility.DisplayDialog(ScriptName, "Update complete!", "Ok");
                    UpdaterConfigChange(branch);
                    naxokitDashboard.userIsUptoDate = true;
                };
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog(ScriptName, e.Message, "ok");
            }
            return Task.CompletedTask;
        }

        private static void UpdaterConfigChange(Enum branch)
        {
            Config.Url = LatestVersion?.Url;
            Config.Version = LatestVersion?.Version;
            Config.Branch = (NaxoVersionData.BranchType)(LatestVersion?.Branch ?? branch);
            Config.Commit = LatestVersion?.Commit;
            Config.CommitUrl = LatestVersion?.CommitUrl;
            Config.CommitDate = LatestVersion?.CommitDate;
            Config.UpdateConfig();
        }
    }
}