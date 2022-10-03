using SSOService.Models.DTOs.Resource;
using System.Threading.Tasks;

namespace SSOService.Services.General.Interfaces
{
    public interface IRouter
    {
        Task<bool> Route(DestinationDefinitionDTO destinationDefinition, string data = null);

    }
}
