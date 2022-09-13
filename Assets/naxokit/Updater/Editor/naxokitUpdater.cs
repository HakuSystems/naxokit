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

        public static readonly string CurrentVersion = Config.Version;
        private  static NaxoVersionData LatestVersion { get; set; }
        
        private List<NaxoVersionData> _versionList;
        
        public static async Task CheckForUpdates(Enum branch = null)
        {
            if(branch == null)
                branch = Config.Branch;
            LatestVersion = await GetLatestVersion(branch);
            if (CurrentVersion != LatestVersion.Version)
            {
                var update = EditorUtility.DisplayDialog(ScriptName,
                    "Version V"+LatestVersion.Version+" is available. Do you want to update?", "Yes", "No");
                if (update)
                {
                    BackupManager.CreateBackup(Config.BackupManager_SaveAsUnitypackage_Enabled,
                        Config.BackupManager_DeleteOldBackups_Enabled);
                    await  DownloadLatestReleaseVersion();
                }
                naxokitDashboard.UserIsUptoDate = false;
            }
            naxokitDashboard.UserIsUptoDate = true;
        }

        private static async Task<NaxoVersionData> GetLatestVersion(Enum branch)
        {
            if(branch.Equals(NaxoVersionData.BranchType.Release))
                return await GetReleaseVersion();
            return await GetBetaVersion();
        }

        private static async Task<NaxoVersionData> GetBetaVersion()
        {
            var response = await HttpClient.GetAsync(VersionListUri);
            var content = await response.Content.ReadAsStringAsync();
            var versionList = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<List<NaxoVersionData>>>(content);
            System.Diagnostics.Debug.Assert(versionList != null, nameof(versionList) + " != null");
            var latestVersion = versionList.Data.OrderByDescending(x => x.Version).FirstOrDefault(x=> x.Branch.Equals(NaxoVersionData.BranchType.Beta));
            return latestVersion;
        }

        private static async Task<NaxoVersionData> GetReleaseVersion()
        {
            var response = HttpClient.GetAsync(ReleaseVersionUri);
            var content = await response.Result.Content.ReadAsStringAsync();
            var versionResponse = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<NaxoVersionData>>(content);
            return versionResponse?.Data;
        }
        
        private static Task DownloadLatestReleaseVersion()
        {
            var results = 0f;
            try
            {
                WebClient.DownloadFileAsync(new Uri(LatestVersion?.Url ?? "Invalid"),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    $"/naxokit V{LatestVersion?.Version}.unitypackage");
                WebClient.DownloadProgressChanged += (sender, args) =>
                {
                    EditorUtility.DisplayProgressBar("Downloading",
                        $"Downloading naxokit V{LatestVersion?.Version}", args.ProgressPercentage);
                };
                WebClient.DownloadFileCompleted += (sender, args) =>
                {
                    EditorUtility.ClearProgressBar();

                    EditorUtility.DisplayProgressBar(ScriptName, "Deleting old files", results);
                    var files = Directory.GetFiles(Application.dataPath + "/naxokit", "*.*", SearchOption.AllDirectories)
                        .Where(s => !s.EndsWith("naxokitUpdater.cs"));
                    
                    
                    var alleles = files as string[] ?? files.ToArray();
                    foreach (var file in alleles)
                    {
                        //File.Delete(file);
                        results++;
                        EditorUtility.DisplayProgressBar(ScriptName, "Deleting old files", (float)results / alleles.Count());
                    }
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.Refresh();
                    
                    //AssetDatabase.ImportPackage(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+$"/naxokit V{LatestVersion?.Version}.unitypackage", false);
                    
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                $"/naxokit V{LatestVersion?.Version}.unitypackage");
                    UpdaterConfigChange();
                };
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog(ScriptName, e.Message, "ok");
            }

            return Task.CompletedTask;
        }

        private static void UpdaterConfigChange()
        {
            Config.Url = LatestVersion?.Url;
            Config.Version = LatestVersion?.Version;
            Config.Branch = (NaxoVersionData.BranchType)LatestVersion?.Branch;
            Config.Commit = LatestVersion?.Commit;
            Config.CommitUrl = LatestVersion?.CommitUrl;
            Config.CommitDate = LatestVersion?.CommitDate;
            Config.UpdateConfig();
        }
    }
}