using naxokit.Helpers.Configs;
using naxokit.Helpers.Logger;
using naxokit.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace naxokit.Screens.Auth
{
    public class naxoApiHelper
    {
        private static readonly HttpClient client = new HttpClient();
        public static ApiData.ApiUser User;

        private const string BASE_URL = "https://api.naxokit.com";
        private static readonly Uri UserSelfUri = new Uri(BASE_URL + "/user/self");
        private static readonly Uri RedeemUri = new Uri(BASE_URL + "/user/redeemables/redeem");
        private static readonly Uri LoginUri = new Uri(BASE_URL + "/user/login");
        private static readonly Uri SignupUri = new Uri(BASE_URL + "/user/signup");
        private static bool running = false;
        private const string AppJson = "application/json";

        public static bool IsLoggedInAndVerified() => IsUserLoggedIn() && User.IsVerified;

        public static bool IsUserLoggedIn()
        {
            if (User == null && !string.IsNullOrEmpty(auth_api.Config.AuthKey) && !running) CheckUserSelf();
            return User != null;
        }


        private static void ClearLogin()
        {
            naxoLog.Log("naxoApiHelper", "Clearing Login Data");
            naxokitDashboard.finallyLoggedIn = false;
            User = null;
            auth_api.Config.AuthKey = null;
            auth_api.Save();
            naxokit.DiscordRPC.naxokitRPC.UpdateRPC();
        }
        private static async Task<HttpResponseMessage> MakeApiCall(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(auth_api.Config.AuthKey))
                request.Headers.Add("Auth-Key", auth_api.Config.AuthKey);

            var response = await client.SendAsync(request);
            var data = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<object>>(await response.Content.ReadAsStringAsync());
            if (!response.IsSuccessStatusCode)
            {
                naxoLog.LogWarning("naxoApiHelper", "Failed to make Api Call: " + data.Message);
                ClearLogin();
            }
            return response;

        }
        private static async void CheckUserSelf()
        {
            running = true;
            naxoLog.Log("naxoApiHelper", "Checking User");
            var request = new HttpRequestMessage(HttpMethod.Get, UserSelfUri);
            var response = await MakeApiCall(request);
            string json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<ApiData.ApiUser>>(json);
            User = data.Data;
            naxoLog.Log("naxoApiHelper", "Sucsessfully got User: " + User.Username);
            naxokitDashboard.SetFinallyLoggedIn(true);
            running = false;
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
            auth_api.Config.AuthKey = data.Data.AuthKey;
            auth_api.Save();
            CheckUserSelf();
            naxokitDashboard.SetFinallyLoggedIn(true);
            naxokit.DiscordRPC.naxokitRPC.UpdateRPC();
        }

        private static void SaveRecivedPassword(string password)
        {
            naxoLog.Log("naxoApiHelper", "Saving Password");
            auth_api.Config.Password = password;
            auth_api.Save();
        }

        public static string GetSavedPassword()
        {
            if (string.IsNullOrEmpty(auth_api.Config.Password))
                return "";
            return auth_api.Config.Password;
        }

        public static void Logout() => ClearLogin();
        public static async void SignUp(string username, string password, string email)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new ApiData.ApiSignupData { Username = username, Password = password, Email = email }), System.Text.Encoding.UTF8, AppJson);
            var request = new HttpRequestMessage(HttpMethod.Post, SignupUri) { Content = content };
            var response = await MakeApiCall(request);
            var data = JsonConvert.DeserializeObject<ApiData.ApiBaseResponse<ApiData.ApiSanityCheckResponse>>(await response.Content.ReadAsStringAsync());
            if (data.Message.Contains("Sanity checks"))
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
            return;
        }

        public static string ApiGenerateStrongPassword()
        {
            int minLenght = 8;
            int maxLenght = 128;
            int minLowerCase = 1;
            int minNumber = 1;
            int minSpecialChar = 1;
            string allowedSpecials = "@#$%/.!'_-";
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

            string password = "";

            int allowedCharsLenght = allowedChars.Length;
            int allowedSpecialsLenght = allowedSpecials.Length;
            int passwordLenght = UnityEngine.Random.Range(minLenght, maxLenght);
            int lowerCaseLetters = UnityEngine.Random.Range(minLowerCase, passwordLenght);
            int numbers = UnityEngine.Random.Range(minNumber, passwordLenght);
            int specialChars = UnityEngine.Random.Range(minSpecialChar, passwordLenght);
            for (int i = 0; i < lowerCaseLetters; i++)
            {
                password += allowedChars[UnityEngine.Random.Range(0, allowedCharsLenght)];
            }
            for (int i = 0; i < numbers; i++)
            {
                password += allowedChars[UnityEngine.Random.Range(0, allowedCharsLenght)];
            }
            for (int i = 0; i < specialChars; i++)
            {
                password += allowedSpecials[UnityEngine.Random.Range(0, allowedSpecialsLenght)];
            }
            char[] passwordArray = password.ToCharArray();
            Random rng = new Random();
            int n = passwordArray.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = passwordArray[k];
                passwordArray[k] = passwordArray[n];
                passwordArray[n] = value;
            }
            naxoLog.Log("naxoApiHelper", "Generated Strong Password!");
            return new string(passwordArray);
        }
    }
}
