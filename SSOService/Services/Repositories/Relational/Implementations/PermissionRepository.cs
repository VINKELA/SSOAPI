using Microsoft.EntityFrameworkCore;
using SSOService.Extensions;
using SSOService.Helpers;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Permission;
using SSOService.Models.DTOs.User;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetPermissionDTO ReturnType = new();
        private readonly GetUserDTO _currentUser = RequestContext.GetCurrentUser;
        public PermissionRepository(SSODbContext db, IServiceResponse serviceResponse)
        {
            _db = db;
            _response = serviceResponse;
        }
        public async Task<Response<GetPermissionDTO>> Create(CreatePermissionDTO permissionDTO)
        {

            var permission = new Permission
            {
                Name = permissionDTO.Name,
                CreatedBy = _currentUser.Email,
                Scope = permissionDTO.Scope,
                PermissionType = permissionDTO.PermissionType,
                ResourceId = permissionDTO.ResourceId
            };
            await _db.AddAsync(permission);
            var status = await _db.SaveChangesAsync() > 0;
            return status ? _response.SuccessResponse(ToDto(permission))
                : _response.FailedResponse(ReturnType);
        }
        public async Task Create(List<CreatePermissionDTO> permissionDTOs)
        {

            var permissions = permissionDTOs.Select(x => new Permission
            {
                Name = x.Name,
                CreatedBy = _currentUser?.Email,
                Scope = x.Scope,
                PermissionType = x.PermissionType,
                ResourceId = x.ResourceId
            });
            await _db.AddRangeAsync(permissions);
            var status = await _db.SaveChangesAsync();
        }

        public async Task<Response<GetPermissionDTO>> Update(long id, UpdatePermissionDTO permissionDTO)
        {

            var currentPermission = _db.Permissions.FirstOrDefault(x => x.PermissionId == id);
            var application = new Permission
            {
                Name = permissionDTO.Name?.ToLower() ?? currentPermission.Name,
                LastModifiedBy = _currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentPermission);
            var status = await _db.SaveChangesAsync() > 0;
            return status ? _response.SuccessResponse(ToDto(currentPermission))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetPermissionDTO>> ChangeState(long id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);

            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Permission));
            if (deactivate) current.IsActive = !deactivate;
            else if (delete)
            {
                current.IsActive = !delete;
                current.IsDeleted = delete;
            }
            else current.IsActive = true;
            var hasChanged = await HasChanged(current);
            if (hasChanged)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, current.PermissionId));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Permissions.Update(current);
            var result = await _db.SaveChangesAsync();
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetPermissionDTO>> Get(long id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetPermissionDTO>>> Get(string name)
        {

            var list = await _db.Permissions.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            return _response.SuccessResponse(list.Select(x => ToDto(x)));
        }

        private static GetPermissionDTO ToDto(Permission permission)
        {
            return new GetPermissionDTO
            {
                Scope = permission.Scope.DisplayName(),
                PermissionType = permission.PermissionType.DisplayName(),
                Name = permission.Name,
                ResourceId = permission.ResourceId
            };
        }
        private async Task<bool> HasChanged(Permission permission)
        {
            var lastest = await Exists(permission.PermissionId);
            return !(permission.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Permission> Exists(long id)
        {
            var current = await _db.Permissions.FirstOrDefaultAsync(x => x.PermissionId == id && !x.IsDeleted);
            return current;
        }

    }
}
