using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;

namespace NewsService.GoogleAuth
{
    public class GoogleAuthenticationHandler : AuthenticationHandler<GoogleAuthenticationOptions>
    {
        public GoogleAuthenticationHandler(IOptionsMonitor<GoogleAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return await Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }

            string authorizationHeader = Request.Headers["Authorization"].ToString();

            string token = authorizationHeader.Split(" ")[1];

            HttpClient _httpClient = new HttpClient();
            string apiUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseBody);
                    UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(responseBody);

                    var identity = new ClaimsIdentity(userInfo.ToClaims(), Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);

                    AuthenticationProperties authenticationProperties = new AuthenticationProperties();

                    return await Task.FromResult(AuthenticateResult.Success(
                        new AuthenticationTicket(principal, authenticationProperties, Scheme.Name)));
                }
                else
                {
                    return await Task.FromResult(AuthenticateResult.Fail("Else Authentication failed."));
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(AuthenticateResult.Fail("Exception Authentication failed." + e));
            }
        }
    }
}
