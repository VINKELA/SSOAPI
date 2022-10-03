using SSOService.Models;
using SSOService.Models.DTOs.Permission;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Response<IEnumerable<GetPermissionDTO>>> Get(string name);
        Task<Response<GetPermissionDTO>> Get(long id);
        Task<Response<GetPermissionDTO>> ChangeState(long id, bool deactivate = false, bool delete = false);
        Task<Response<GetPermissionDTO>> Update(long id, UpdatePermissionDTO permissionDTO);
        Task<Response<GetPermissionDTO>> Create(CreatePermissionDTO permissionDTO);
        Task Create(List<CreatePermissionDTO> permissionDTOs);


    }
}
