using System.Security.Claims;
using System.Text.Json.Serialization;

namespace NewsService.GoogleAuth
{
    public class UserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("verified_email")]
        public bool VerifiedEmail { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }
        [JsonPropertyName("picture")]
        public string Picture { get; set; }
        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        public List<Claim> ToClaims()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, Name));
            claims.Add(new Claim(ClaimTypes.Email, Email));
            return claims;
        }
    }
}
