using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SSOService.IntelligenceProcessors.PowerBusinessIntelligence
{
    public class GetEmbedToken
    {
        public static HttpResponseMessage Execute(string requestUri, object dataObject, string accessToken)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    return httpClient.PostAsJsonAsync(new Uri(requestUri), dataObject).Result;
                }                
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
