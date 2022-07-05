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

//Development Note
/*
 * This code is Yet Only for
 * the old SDK
 * since we have not update the website to
 * naxokit.com
*/
//End


namespace naxokit.Updater
{
    public class naxokitUpdater : MonoBehaviour
    {
        private static string scriptName = "naxokitUpdater";
        
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string _BASE_URL = "https://api.nanosdk.net";

        private static readonly Uri _LatestVersionUri = new Uri(_BASE_URL + $"/public/sdk/version");
        private static readonly Uri _VersionUri = new Uri(_BASE_URL + "/public/sdk/version/list");

        public static string CurrentVersion { get; } = File.ReadAllText($"Assets{Path.DirectorySeparatorChar}naxokit{Path.DirectorySeparatorChar}version.txt").Replace("\n", "");
        public static List<VersionBaseINTERNDATA> ServerVersionList;
        public static VersionBaseINTERNDATA LatestVersion { set; get; }
        public static VersionBaseINTERNDATA LatestBetaVersion { set; get; }


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
                    NanoLog("Download failed!");
                    if (EditorUtility.DisplayDialog(scriptName, "Failed Download: " + ex.Message, "Join Discord for help", "Cancel"))
                    {
                        Application.OpenURL("https://nanosdk.net/discord");
                    }
                    return;
                }
            }
            NanoLog("Download Complete");

            try
            {

                if (EditorUtility.DisplayDialog(scriptName, "The Old Version will Be Deleted and the New one Will be imported!", "Okay", "Cancel"))
                {
                    NanoLog("Getting Files..");
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
                                    NanoLog($"{f} - Deleted");
                                    File.Delete(f);
                                }
                            }
                        }
                        string[] dllDir = Directory.GetFiles(naxoPath, "*.dll", SearchOption.AllDirectories);
                        foreach (string f in dllDir)
                        {
                            try
                            {
                                NanoLog($"{f} - Deleted");
                                File.Delete(f);
                            }
                            catch (Exception) { }
                        }
                    });
                }
                else
                {
                    NanoLog("User declined update");

                    NanoLog("Deleting downloaded file");
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

                NanoLog("Download failed!");
                if (EditorUtility.DisplayDialog(scriptName, "Failed Download: " + ex.Message, "Join Discord for help", "Cancel"))
                {
                    Application.OpenURL("https://nanosdk.net/discord");
                }
            }

            AssetDatabase.Refresh();

        }
        public static async Task UpdateVersionData()
        {
            ServerVersionList = await GetVersionList();
            LatestVersion = await GetLatestVersion(VersionBaseINTERNDATA.ReleaseType.Avatar, VersionBaseINTERNDATA.BranchType.Release);
            LatestBetaVersion = await GetLatestVersion(VersionBaseINTERNDATA.ReleaseType.Avatar, VersionBaseINTERNDATA.BranchType.Beta);
        }
        public static async Task<VersionBaseINTERNDATA> GetLatestVersion(VersionBaseINTERNDATA.ReleaseType type, VersionBaseINTERNDATA.BranchType branch)
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
                    return JsonConvert.DeserializeObject<VersionBaseINTERN<VersionBaseINTERNDATA>>(data).Data;

                }
                NanoLog("Failed to get latest version");
                return null;
            }
        }
        public static async Task<List<VersionBaseINTERNDATA>> GetVersionList()
        {

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = _VersionUri
            };

            using (var response = await HttpClient.SendAsync(request))
            {
                string result = await response.Content.ReadAsStringAsync();
                var SERVERCHECKproperties = JsonConvert.DeserializeObject<VersionBaseINTERN<List<VersionBaseINTERNDATA>>>(result);
                return removeEntries(SERVERCHECKproperties.Data, VersionBaseINTERNDATA.ReleaseType.World);
            }

        }
        private static List<VersionBaseINTERNDATA> removeEntries(List<VersionBaseINTERNDATA> list, VersionBaseINTERNDATA.ReleaseType release)
        {
            List<VersionBaseINTERNDATA> newList = new List<VersionBaseINTERNDATA>();
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
        private static void NanoLog(string message)
        {
            message = "<color=magenta>" + message + "</color>";
            Debug.Log(scriptName+": " + message);
        }
    }

    public class VersionBaseINTERNDATA
    {
        public string Url { get; set; }
        public string Version { get; set; }
        public ReleaseType Type { get; set; }

        public BranchType Branch { get; set; }

        public enum ReleaseType
        {
            Avatar = 0,
            World = 1
        }

        public enum BranchType
        {
            Release = 0,
            Beta = 1
        }
    }

    public class VersionBaseINTERN<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
