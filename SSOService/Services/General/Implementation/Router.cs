namespace SSOService.Services.General.Implementation
{
    using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SSOService.Models;
using SSOService.Models.DTOs.Resource;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
namespace PowerTrackEnterprise.Core.ProcessingMethods.ProcessDestination
{
    public class Router
    {
            readonly ILogger<Router> _logger;
            public Router(ILogger<Router> logger)
            {
                _logger = logger;
            }

            public  bool Route(DestinationDefinitionDTO destinationDefinition, string data)
            {     
                try
            {
                using (var client = new HttpClient())
                {
                    _logger.LogInformation(
                        $"Processing Push Instruction ==> {JsonConvert.SerializeObject(destinationDefinition, Formatting.Indented)}");

                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(destinationDefinition.EndPointUrl),
                        Method = new HttpMethod(destinationDefinition.EndPointMethod),
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(data)),
                    };

                    foreach (var customHeader in destinationDefinition.CustomHeaders.Where(x =>
                        !string.IsNullOrEmpty(x.Name)))
                        request.Headers.Add(customHeader.Name.ToUpper(), customHeader.Value);
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var result = client.SendAsync(request).Result;
                    var resultData = result.Content.ReadAsStringAsync().Result;

                    _logger.LogInformation("INFO", resultData);

                    return JsonConvert
                        .DeserializeObject<Response<object>>(resultData).Status;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
    }
}}
