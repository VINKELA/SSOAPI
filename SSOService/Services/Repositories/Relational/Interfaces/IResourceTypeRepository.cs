using SSOService.Models;
using SSOService.Models.DTOs.ServiceType;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Interfaces
{
    public interface IResourceType
    {
        Task<Response<IEnumerable<GetServiceTypeDTO>>> Get(string name);
        Task<Response<GetServiceTypeDTO>> Get(Guid id);
        Task<Response<GetServiceTypeDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Task<Response<GetServiceTypeDTO>> Update(Guid id, UpdateServiceTypeDTO serviceTypeDTO);
        Task<Response<GetServiceTypeDTO>> Create(CreateServiceTypeDTO serviceTypeDTO);

    }
}
