using SSOService.Models;
using SSOService.Models.DTOs.ServiceType;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Interfaces
{
    public interface IResourceType
    {
        Task<Response<IEnumerable<GetResourceTypeDTO>>> Get(string name);
        Task<Response<GetResourceTypeDTO>> Get(Guid id);
        Task<Response<GetResourceTypeDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Task<Response<GetResourceTypeDTO>> Update(Guid id, UpdateResourceTypeDTO serviceTypeDTO);
        Task<Response<GetResourceTypeDTO>> Create(CreateResourceTypeDTO serviceTypeDTO);

    }
}
