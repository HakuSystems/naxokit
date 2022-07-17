using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Net.Http;
using System.Net;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using naxokit.Helpers.Models;
using naxokit.Helpers.Logger;

//Development Note
/*
 * We have now updated the Website, but we need to update the api someday
 * so we wont have  "Avatar, World" anymore etc.
*/
//End


namespace naxokit.Updater
{
    public class naxokitUpdater : MonoBehaviour
    {
        private static string scriptName = "naxokitUpdater";
        
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string _BASE_URL = "https://api.naxokit.com";

        private static readonly Uri _LatestVersionUri = new Uri(_BASE_URL + $"/public/sdk/version");
        private static readonly Uri _VersionUri = new Uri(_BASE_URL + "/public/sdk/version/list");

        public static string CurrentVersion { get; } = File.ReadAllText($"Assets{Path.DirectorySeparatorChar}naxokit{Path.DirectorySeparatorChar}version.txt").Replace("\n", "");
        public static List<VersionData> ServerVersionList;
        public static VersionData LatestVersion { set; get; }
        public static VersionData LatestBetaVersion { set; get; }


        //select where to be imported
        private static string assetPath = $"Assets{Path.DirectorySeparatorChar}";
        //Custom name for downloaded unitypackage
        private static string assetName = "unitypackage";
        //gets VRCSDK Directory Path
        private static string naxoPath = $"Assets{Path.DirectorySeparatorChar}naxokit{Path.DirectorySeparatorChar}";


        //[MenuItem("naxokit/Update Test", false, 500)]
        public static async void DeleteAndDownloadAsync(string version = "latest")
        {
            using (WebClient w = new WebClient())
            {
                w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
                w.DownloadProgressChanged += FileDownloadProgress;
                try
                {
                    string url = await GetUrlFromVersion(version);
                    if (url == null) throw new Exception("Invalid version");
                    await w.DownloadFileTaskAsync(new Uri(url), Path.GetTempPath() + Path.DirectorySeparatorChar + $"{version}.{assetName}");
                }
                catch (Exception ex)
                {
                    naxoLog.LogError("Updater","Download failed!");
                    if (EditorUtility.DisplayDialog(scriptName, "Failed Download: " + ex.Message, "Join Discord for help", "Cancel"))
                    {
                        Application.OpenURL("https://naxokit.com/discord");
                    }
                    return;
                }
            }
            naxoLog.Log("Updater","Download Complete");

            try
            {

                if (EditorUtility.DisplayDialog(scriptName, "The Old Version will Be Deleted and the New one Will be imported!", "Okay", "Cancel"))
                {
                    naxoLog.Log("Updater", "Getting Files..");
                    string[] naxoDir = Directory.GetFiles(naxoPath, "*.*", SearchOption.AllDirectories);


                    Debug.Log("Deleting Files...");

                    
                    await Task.Run(() =>
                    {
                        foreach (string f in naxoDir)
                        {
                            if (!f.Contains(scriptName + ".cs"))
                            {
                                if (!IsDLLFile(f))
                                {
                                    naxoLog.Log("Updater", $"{f} - Deleted");
                                    File.Delete(f);
                                }
                            }
                        }
                        string[] dllDir = Directory.GetFiles(naxoPath, "*.dll", SearchOption.AllDirectories);
                        foreach (string f in dllDir)
                        {
                            try
                            {
                                naxoLog.Log("Updater", $"{f} - Deleted");
                                File.Delete(f);
                            }
                            catch (Exception) { }
                        }
                    });
                }
                else
                {
                    naxoLog.Log("Updater", "User declined update");

                    naxoLog.Log("Updater", "Deleting downloaded file");
                    File.Delete(Path.GetTempPath() + Path.DirectorySeparatorChar + $"{version}.{assetName}");
                    return;

                }
            }
            catch (DirectoryNotFoundException)
            {
                EditorUtility.DisplayDialog(scriptName, "Directory Not Found", "Okay");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog(scriptName, "Failed to delete files, please delete them manually: "+ex.Message, "Okay");
                return;
            }

            try
            {
                AssetDatabase.ImportPackage(Path.GetTempPath() + Path.DirectorySeparatorChar + $"{version}.{assetName}", false);

            }
            catch (Exception ex)
            {

                naxoLog.LogWarning("Updater", "Download failed!");
                if (EditorUtility.DisplayDialog(scriptName, "Failed Download: " + ex.Message, "Join Discord for help", "Cancel"))
                {
                    Application.OpenURL("https://naxokit.com/discord");
                }
            }

            AssetDatabase.Refresh();

        }
        public static async Task UpdateVersionData()
        {
            ServerVersionList = await GetVersionList();
            LatestVersion = await GetLatestVersion(VersionData.ReleaseType.Avatar, VersionData.BranchType.Release);
            LatestBetaVersion = await GetLatestVersion(VersionData.ReleaseType.Avatar, VersionData.BranchType.Beta);
        }
        public static bool CompareCurrentVersionWithLatest()
        {
            var splittedCurrentVersion = CurrentVersion.Split(';');
            var splittedCurrentVersionIndex = splittedCurrentVersion[0];
            if (splittedCurrentVersionIndex == LatestVersion.Version)
                return true;
            else
                return false;
        }
        public static async Task<VersionData> GetLatestVersion(VersionData.ReleaseType type, VersionData.BranchType branch)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_LatestVersionUri + $"?type={type}&branch={branch}")
            };
            using (var response = await HttpClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<VersionBase<VersionData>>(data).Data;

                }
                naxoLog.LogWarning("Updater", "Failed to get latest version");
                return null;
            }
        }
        public static async Task<List<VersionData>> GetVersionList()
        {

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = _VersionUri
            };

            using (var response = await HttpClient.SendAsync(request))
            {
                string result = await response.Content.ReadAsStringAsync();
                var SERVERCHECKproperties = JsonConvert.DeserializeObject<VersionBase<List<VersionData>>>(result);
                return removeEntries(SERVERCHECKproperties.Data, VersionData.ReleaseType.World);
            }

        }
        private static List<VersionData> removeEntries(List<VersionData> list, VersionData.ReleaseType release)
        {
            List<VersionData> newList = new List<VersionData>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Type == release) continue;
                newList.Add(list[i]);
            }
            return newList;
        }
        private async static Task<string> GetUrlFromVersion(string version)
        {
            ServerVersionList = await GetVersionList();
            string url = null;
            if (version.Equals("latest")) url = ServerVersionList[0].Url;
            else if (version.Equals("beta")) url = ServerVersionList[ServerVersionList.Count - 1].Url;

            for (int i = 0; i < ServerVersionList.Count; i++)
            {
                if (version.Equals(ServerVersionList[i].Version)) url = ServerVersionList[i].Url;
            }
            return url;
        }
        private static void FileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            EditorUtility.DisplayProgressBar(scriptName, "Downloading...", e.ProgressPercentage / 100f);
        }
        private static bool IsDLLFile(string path)
        {
            if (path.Substring(path.Length - 3).Equals("dll")) return true;
            return false;
        }
        
    }
}
