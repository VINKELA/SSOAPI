using SSOService.Models;
using SSOService.Models.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IServiceRepository
    {
        Task<Response<IEnumerable<GetServiceDTO>>> Get(string name);
        Task<Response<GetServiceDTO>> Get(Guid id);
        Task<Response<GetServiceDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Task<Response<GetServiceDTO>> Update(Guid id, UpdateServiceDTO serviceDTO);
        Task<Response<GetServiceDTO>> Create(CreateServiceDTO serviceDTO);
    }
}
