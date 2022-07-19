using JetBrains.Annotations;
using naxokit.Helpers.Logger;
using Newtonsoft.Json;
using System;
using System.IO;

namespace naxokit.Helpers.Configs
{
    /*#0003 Save Json Password
             * 
             * Implement Json Password Saving since our Auth-Key gets invalid
             * after some time.
             * 
             * Also known as "remember me"
             * 
            */

    public class auth_api
    {
        private static NaxoConf _internalConfig;
        private static readonly string AppdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string AppdataNaxoFolder = Path.Combine(AppdataFolder, "naxokit");
        private static readonly string JsonPath = Path.Combine(AppdataNaxoFolder, "auth_api.json");

        public static NaxoConf Config
        {
            get
            {
                TryLoad();
                return _internalConfig;
            }
        }
        static auth_api() => TryLoad();
        private static void TryLoad()
        {
            if (File.Exists(JsonPath))
            {
                var json = File.ReadAllText(JsonPath);
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        _internalConfig = JsonConvert.DeserializeObject<NaxoConf>(json);
                    }
                    catch (Exception ex)
                    {
                        naxoLog.LogError("auth_api", ex.Message);
                    }
                }
            }
            if (_internalConfig != null) return;
            _internalConfig = new NaxoConf();
            Save();
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText(JsonPath, JsonConvert.SerializeObject(_internalConfig));
            }
            catch (Exception ex)
            {
                naxoLog.LogError("auth_api", ex.Message);
            }
        }

    }

    public class NaxoConf
    {
        [CanBeNull] public string AuthKey { get; set; }
    }
}
