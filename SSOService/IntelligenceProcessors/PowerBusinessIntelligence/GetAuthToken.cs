using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace SSOService.IntelligenceProcessors.PowerBusinessIntelligence
{
    public static class GetAuthToken
    {
        public static HttpResponseMessage Execute(string requestUri, string username, string password, string clientId)
        {
            try
            {
                var ssoConfigContent = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("resource", "https://analysis.windows.net/powerbi/api"),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (HttpClient httpClient = new HttpClient())
                {
                    return httpClient.PostAsync(new Uri(requestUri), ssoConfigContent).Result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
