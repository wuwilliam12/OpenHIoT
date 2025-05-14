using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.Client.Requests
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class OAuthClient
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetAccessTokenAsync(string tokenEndpoint, string clientId, string clientSecret, string scope)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            var requestBody = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("scope", scope)
        });

            request.Content = requestBody;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(responseBody);

            return tokenResponse.AccessToken;
        }
        public static async Task<string> MakeAuthenticatedRequestAsync(string apiUrl, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
    }


}
