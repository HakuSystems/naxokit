using System.Collections.Generic;

namespace naxokit.Helpers.Models
{
    public class ApiData
    {
        public class ApiUser
        {
            public string ID { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public Permissions Permission { get; set; }
            public bool IsVerified { get; set; }
            public bool IsPremium { get; set; }
        }
        public enum Permissions
        {
            User = 0,
            Moderator = 5,
            Admin = 10,
            System = 127
        }
        public class ApiSignupData
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public class ApiLoginData
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class ApiBaseResponse<T>
        {
            public string Message { get; set; }
            public T Data { get; set; }
        }

        public class ApiSanityCheckResponse
        {
            public Dictionary<string, string> UsernameSanityCheck { get; set; }
            public Dictionary<string, string> PasswordSanityCheck { get; set; }
            public Dictionary<string, string> EmailSanityCheck { get; set; }
        }

        public class ApiLoginResponse
        {
            public string AuthKey { get; set; }
        }

        public class ApiLicenseData
        {
            public string Key { get; set; }
        }
    }
}
