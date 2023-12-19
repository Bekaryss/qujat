using Newtonsoft.Json;
using Qujat.Core.Data.Entities;

namespace Qujat.Backoffice.Api.Models
{
    public class AdminDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("userType")]
        public ApplicationUserType UserType { get; set; }
    }

    public class SignInAdminUserRq
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public record SignInAdminUserRp
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }


    public class SignUpAdminRq
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class ResetPasswordForAdminRq
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
