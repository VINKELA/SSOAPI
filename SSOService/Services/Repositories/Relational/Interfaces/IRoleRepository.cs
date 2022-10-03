using SSOService.Models;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IRoleRepository
    {
        Task<Response<IEnumerable<GetRoleDTO>>> Get(string name);
        Task<Response<GetRoleDTO>> Get(long id);
        Task<Response<GetRoleDTO>> ChangeState(long id, bool deactivate = false, bool delete = false);
        Task<Response<GetRoleDTO>> Update(long id, UpdateRoleDTO roleDTO);
        Task<Response<GetRoleDTO>> Create(CreateRoleDTO roleDTO);
    }
}
