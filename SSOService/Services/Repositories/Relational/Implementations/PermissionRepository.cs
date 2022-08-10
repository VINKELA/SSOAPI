using Microsoft.EntityFrameworkCore;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Permission;
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
        private readonly IUserRepository _userRepository;
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetPermissionDTO ReturnType = new();
        public PermissionRepository(IUserRepository userRepository, SSODbContext db,
            IServiceResponse serviceResponse)
        {
            _userRepository = userRepository;
            _db = db;
            _response = serviceResponse;
        }
        public async Task<Response<GetPermissionDTO>> Create(CreatePermissionDTO permissionDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var application = new Permission
            {
                Name = permissionDTO.Name,
                CreatedBy = currentUser.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetPermissionDTO>> Update(Guid id, UpdatePermissionDTO permissionDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var currentPermission = _db.Permissions.FirstOrDefault(x => x.Id == id);
            var application = new Permission
            {
                Name = permissionDTO.Name?.ToLower() ?? currentPermission.Name,
                LastModifiedBy = currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentPermission);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(currentPermission))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetPermissionDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            var currentUser = _userRepository.GetLoggedInUser();
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Permission));
            if (deactivate) current.IsActive = !deactivate;
            else if (delete)
            {
                current.IsActive = !delete;
                current.IsDeleted = delete;
            }
            else current.IsActive = true;
            var hasChanged = await HasChanged(current);
            if (hasChanged)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, current.Id));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Permissions.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(currentUser.Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetPermissionDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetPermissionDTO>>> Get(string name)
        {
            var user = _userRepository.GetLoggedInUser();
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
                ClientId = permission.ClientId,
                Scope = permission.Scope.DisplayName(),
                Entity = permission.Entity.DisplayName(),
                PermissionType = permission.PermissionType.DisplayName(),
            };
        }
        private async Task<bool> HasChanged(Permission permission)
        {
            var lastest = await Exists(permission.Id);
            return !(permission.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Permission> Exists(Guid id)
        {
            var current = await _db.Permissions.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }
}
