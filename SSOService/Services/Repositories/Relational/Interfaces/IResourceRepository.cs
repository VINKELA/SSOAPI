using SSOService.Models;
using SSOService.Models.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IResourceRepository
    {
        Task<Response<IEnumerable<GetResourceDTO>>> Get(string name);
        Task<Response<GetResourceDTO>> Get(Guid id);
        Task<Response<GetResourceDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Task<Response<GetResourceDTO>> Update(Guid id, UpdateResourceDTO serviceDTO);
        Task<Response<GetResourceDTO>> Create(CreateResourceDTO serviceDTO);
    }
}
