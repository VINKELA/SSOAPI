using SSOService.Models;
using SSOService.Models.DTOs.ReSourceType;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Interfaces
{
    public interface IServiceType
    {
        Task<Response<IEnumerable<GetServiceTypeDTO>>> Get(string name);
        Task<Response<GetServiceTypeDTO>> Get(long id);
        Task<Response<GetServiceTypeDTO>> ChangeState(long id, bool deactivate = false, bool delete = false);
        Task<Response<GetServiceTypeDTO>> Update(long id, UpdateServiceTypeDTO serviceTypeDTO);
        Task<Response<GetServiceTypeDTO>> Create(CreateServiceTypeDTO serviceTypeDTO);

    }
}
