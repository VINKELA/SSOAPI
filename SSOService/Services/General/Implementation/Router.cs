namespace SSOService.Services.General.Implementation
{
    using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SSOService.Models;
using SSOService.Models.DTOs.Resource;
    using SSOService.Models.Enums;
    using SSOService.Services.General.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
using System.Net.Http;
using System.Text;
    using System.Threading.Tasks;
    using System.Web;

namespace PowerTrackEnterprise.Core.ProcessingMethods.ProcessDestination
{
    public class Router: IRouter
        {
            readonly ILogger<Router> _logger;
            public Router(ILogger<Router> logger)
            {
                _logger = logger;
            }

            public async  Task<bool> Route(DestinationDefinitionDTO destinationDefinition, string data = null)
            {     
                try
                {
                    using var client = new HttpClient();
                    _logger.LogInformation(
                        $"Processing Push Instruction ==> {JsonConvert.SerializeObject(destinationDefinition, Formatting.Indented)}");
                    var urlParameters = destinationDefinition.CustomParameters
                        .Where(x => x.CustomParameterType == CustomParameterType.url);
                    var url = urlParameters.Any() ? HttpUtility.UrlEncode(destinationDefinition.EndPointUrl +
                                urlParameters.Select(x => $"\\{x.Value}")) : destinationDefinition.EndPointUrl;
                    var queryParameters =destinationDefinition.CustomParameters
                        .Where(x => x.CustomParameterType == CustomParameterType.query);
                    url = queryParameters.Any() ? HttpUtility.UrlEncode(url + "?" + queryParameters
                        .Select(x => $"&&{x.Name}={x.Value}")) : url;
                    var bodyParameters = destinationDefinition.CustomParameters
                        .Where(x => x.CustomParameterType == CustomParameterType.body);
                    var serialized = bodyParameters.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));   // This is where your content gets added to the request body
                    var postData = data != null?  new ByteArrayContent(Encoding.UTF8.GetBytes(data)) 
                        : (serialized.Any()? new FormUrlEncodedContent(serialized): null);
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(url),
                        Method = new HttpMethod(destinationDefinition.EndPointMethod),
                        Content = postData
                    };
                    foreach (var customHeader in destinationDefinition.CustomParameters.Where(x =>
                        !string.IsNullOrEmpty(x.Name) && x.CustomParameterType == CustomParameterType.header))
                        request.Headers.Add(customHeader.Name.ToUpper(), customHeader.Value);
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var result = await client.SendAsync(request);
                    var resultData = await result.Content.ReadAsStringAsync();

                    _logger.LogInformation("INFO", resultData);
                    return JsonConvert
                        .DeserializeObject<Response<object>>(resultData).Status;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return false;
                }
        }
    }
}}
