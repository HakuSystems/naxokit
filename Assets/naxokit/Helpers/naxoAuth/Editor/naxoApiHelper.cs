using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using naxokit.Screens;
using UnityEditor;
using Random = System.Random;

namespace naxokit.Helpers.Auth
{
    public class naxoApiHelper
    {
        private static readonly HttpClient Client = new HttpClient();
        public static ApiData.ApiUser user;

        private const string BaseURL = "https://api.naxokit.com";
        private static readonly Uri UserSelfUri = new Uri(BaseURL + "/user/self");
        private static readonly Uri RedeemUri = new Uri(BaseURL + "/user/redeemables/redeem");
        private static readonly Uri LoginUri = new Uri(BaseURL + "/user/login");
        private static readonly Uri SignupUri = new Uri(BaseURL + "/user/signup");
        private static bool _running = false;
        private const string AppJson = "application/json";

        public static bool IsLoggedInAndVerified() => IsUserLoggedIn() && user.IsVerified;

        public static bool IsUserLoggedIn()
        {
            if (user == null && !string.IsNullOrEmpty(Config.AuthKey) && !_running) CheckUserSelf();
            return user != null;
        }


        private static void ClearLogin()
        {
            naxoLog.Log("naxoApiHelper", "Clearing Login Data");
            naxokitDashboard.finallyLoggedIn = false;
            user = null;
            Config.AuthKey = null;
            Config.UpdateConfig();
            DiscordRPC.naxokitRPC.UpdateRPC();
        }
        private static async Task<HttpResponseMessage> MakeApiCall(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(Config.AuthKey))
                request.Headers.Add("Auth-Key", Config.AuthKey);

            var response = await Client.SendAsync(request);
            var data = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<object>>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode) return response;
            naxoLog.LogWarning("naxoApiHelper", "Failed to make Api Call: " + data.Message);
            ClearLogin();
            return response;

        }
        private static async void CheckUserSelf()
        {
            _running = true;
            naxoLog.Log("naxoApiHelper", "Checking User");
            var request = new HttpRequestMessage(HttpMethod.Get, UserSelfUri);
            var response = await MakeApiCall(request);
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<ApiData.ApiUser>>(json);
            if (data != null) user = data.Data;
            naxoLog.Log("naxoApiHelper", "Sucsessfully got User: " + user.Username);
            naxokitDashboard.SetFinallyLoggedIn(true);
            Config.IsPremiumBoolSinceLastCheck = user.IsPremium;
            Config.LastPremiumCheck = DateTime.Now;
            Config.UpdateConfig();
            _running = false;
        }
        public static async void RedeemLicense(string code)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new ApiData.ApiLicenseData { Key = code }), System.Text.Encoding.UTF8, AppJson);
            var request = new HttpRequestMessage(HttpMethod.Post, RedeemUri) { Content = content };
            var response = await MakeApiCall(request);
            var data = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<object>>(await response.Content.ReadAsStringAsync());
            if (!response.IsSuccessStatusCode)
            {
                naxoLog.LogWarning("naxoApiHelper", "License Redeem Failed: " + data.Message);
                EditorUtility.DisplayDialog("naxoApiHelper", data.Message, "OK");
                return;
            }
            naxoLog.Log("naxoApiHelper", "License Redeemed Successfully");
            EditorUtility.DisplayDialog("naxoApiHelper", "License Redeemed Successfully", "OK");
            CheckUserSelf();
        }
        public static async void Login(string username, string password)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new ApiData.ApiLoginData { Username = username, Password = password }), System.Text.Encoding.UTF8, AppJson);
            var request = new HttpRequestMessage(HttpMethod.Post, LoginUri) { Content = content };
            var response = await MakeApiCall(request);
            var data = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<ApiData.ApiLoginResponse>>(await response.Content.ReadAsStringAsync());
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                naxoLog.LogWarning("naxoApiHelper", "Credentials cant be empty");
            if (!response.IsSuccessStatusCode)
            {
                naxoLog.LogWarning("naxoApiHelper", "Login Failed: " + data.Message);
                EditorUtility.DisplayDialog("naxoApiHelper", data.Message, "OK");
                return;
            }
            naxoLog.Log("naxoApiHelper", "Login Successful");
            if (naxokitDashboard.savePasswordLocally)
                SaveRecivedPassword(password);
            Config.AuthKey = data.Data.AuthKey;
            Config.UpdateConfig();
            CheckUserSelf();
            naxokitDashboard.SetFinallyLoggedIn(true);
            naxokit.DiscordRPC.naxokitRPC.UpdateRPC();
            Config.TermsPolicyAccepted = true;
        }

        private static void SaveRecivedPassword(string password)
        {
            naxoLog.Log("naxoApiHelper", "Saving Password");
            Config.Password = password;
            Config.UpdateConfig();
        }

        public static string GetSavedPassword()
        {
            if (string.IsNullOrEmpty(Config.Password))
                return "";
            return Config.Password;
        }

        public static void Logout() => ClearLogin();
        public static async void SignUp(string username, string password, string email)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new ApiData.ApiSignupData { Username = username, Password = password, Email = email }), System.Text.Encoding.UTF8, AppJson);
            var request = new HttpRequestMessage(HttpMethod.Post, SignupUri) { Content = content };
            var response = await MakeApiCall(request);
            var data = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<ApiData.ApiSanityCheckResponse>>(await response.Content.ReadAsStringAsync());
            if (data != null && data.Message.Contains("Sanity checks"))
            {
                var sb = new StringBuilder();
                string usernameArray = null;
                string passwordArray = null;
                string emailArray = null;
                foreach (var item in data.Data.UsernameSanityCheck)
                {
                    sb.AppendLine(item.Value);
                    usernameArray = sb.ToString();
                }
                sb.Clear();
                foreach (var item in data.Data.PasswordSanityCheck)
                {
                    sb.AppendLine(item.Value);
                    passwordArray = sb.ToString();
                }
                sb.Clear();
                foreach (var item in data.Data.EmailSanityCheck)
                {
                    sb.AppendLine(item.Value);
                    emailArray = sb.ToString();
                }
                if (string.IsNullOrEmpty(usernameArray))
                {
                    usernameArray = "No Errors";
                }
                if (string.IsNullOrEmpty(passwordArray))
                {
                    passwordArray = "No Errors";
                }
                if (string.IsNullOrEmpty(emailArray))
                {
                    emailArray = "No Errors";
                }
                EditorUtility.DisplayDialog("naxoApiHelper", "Signup Failed: " + data.Message + "\n\nUsername Errors:\n" + usernameArray + "\n\nPassword Errors:\n" + passwordArray + "\n\nEmail Errors:\n" + emailArray, "OK");

            }
            if (!response.IsSuccessStatusCode)
            {
                naxoLog.LogWarning("naxoApiHelper", "Signup Failed: " + data.Message);
                return;
            }
            naxoLog.Log("naxoApiHelper", "Signup Successful");
            EditorUtility.DisplayDialog("naxoApiHelper", "Signup Successful", "OK");
        }

        public static string ApiGenerateStrongPassword()
        {
            const int minLenght = 8;
            const int maxLenght = 128;
            const int minLowerCase = 1;
            const int minNumber = 1;
            const int minSpecialChar = 1;
            const string allowedSpecials = "@#$%/.!'_-";
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

            var password = "";

            var allowedCharsLenght = allowedChars.Length;
            var allowedSpecialsLenght = allowedSpecials.Length;
            var passwordLenght = UnityEngine.Random.Range(minLenght, maxLenght);
            var lowerCaseLetters = UnityEngine.Random.Range(minLowerCase, passwordLenght);
            var numbers = UnityEngine.Random.Range(minNumber, passwordLenght);
            var specialChars = UnityEngine.Random.Range(minSpecialChar, passwordLenght);
            for (var i = 0; i < lowerCaseLetters; i++)
            {
                password += allowedChars[UnityEngine.Random.Range(0, allowedCharsLenght)];
            }
            for (var i = 0; i < numbers; i++)
            {
                password += allowedChars[UnityEngine.Random.Range(0, allowedCharsLenght)];
            }
            for (var i = 0; i < specialChars; i++)
            {
                password += allowedSpecials[UnityEngine.Random.Range(0, allowedSpecialsLenght)];
            }
            var passwordArray = password.ToCharArray();
            var rng = new Random();
            var n = passwordArray.Length;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (passwordArray[k], passwordArray[n]) = (passwordArray[n], passwordArray[k]);
            }
            naxoLog.Log("naxoApiHelper", "Generated Strong Password!");
            return new string(passwordArray);
        }
    }
}
