using SSOService.Models;
using SSOService.Models.DTOs.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IEntityRepository
    {
        Task<Response<IEnumerable<GetEntityDTO>>> Get(string name);
        Task<Response<GetEntityDTO>> Get(Guid id);
        Task<Response<GetEntityDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Task<Response<GetEntityDTO>> Update(Guid id, UpdateEntityDTO entityDTO);
        Task<Response<GetEntityDTO>> Create(CreateEntityDTO entityDTO);
    }
}
