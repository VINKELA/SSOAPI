using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.Role;
using SSOService.Models.DTOs.User;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly GetRoleDTO ReturnType = new();
        public RoleRepository(SSODbContext db,
            IServiceResponse serviceResponse, IPermissionRepository permissionRepository, IHttpContextAccessor httpContext)
        {
            _db = db;
            _response = serviceResponse;
            _permissionRepository = permissionRepository;
            _httpContext = httpContext;
        }
        public async Task<Response<GetRoleDTO>> Create(CreateRoleDTO roleDTO)
        {
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var application = new Role
            {
                Name = roleDTO.Name,
                ClientId = roleDTO.ClientId,
                CreatedBy = currentUser.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetRoleDTO>> Update(Guid id, UpdateRoleDTO roleDTO)
        {
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var currentRole = _db.Roles.FirstOrDefault(x => x.Id == id);
            var application = new Role
            {
                Name = roleDTO.Name?.ToLower() ?? currentRole.Name,
                LastModifiedBy = currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentRole);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(currentRole))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetRoleDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Role));
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
            _db.Roles.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(currentUser.Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetRoleDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetRoleDTO>>> Get(string name)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var list = await _db.Roles.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            return _response.SuccessResponse(list.Select(x => ToDto(x)));
        }
        public async Task<Response<GetRoleDTO>> AddClaim(CreateRoleClaim roleClaim)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];

            var role = await Exists(roleClaim.RoleId);

            if (role == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.RoleClaim));
            var newclaim = new RoleClaim
            {
                ClaimType = roleClaim.ClaimType,
                RoleId = roleClaim.RoleId,
                ClaimValue = roleClaim.ClaimValue
            };
            await _db.AddAsync(newclaim);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(role));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetRoleDTO>> UpdateClaim(Guid claimId, Guid roleId, bool update)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var current = await _db.RoleClaims.FirstOrDefaultAsync(x => x.Id == claimId && x.RoleId == roleId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Role));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(roleId)));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetRoleDTO>> AddPermission(Guid permissionId, Guid roleId)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];

            var permission = await _permissionRepository.Get(permissionId);
            var role = await Exists(roleId);

            if (!permission.Status)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Permission));
            if (role == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Role));
            var newAuth = new RolePermission
            {
                PermissionId = permissionId,
                RoleId = roleId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(role));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetRoleDTO>> UpdateRolePermission(Guid permissionId, Guid roleId, bool update)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var current = await _db.RolePermissions.FirstOrDefaultAsync(x => x.PermissionId == permissionId && x.RoleId == roleId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Role));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(roleId)));
            return _response.FailedResponse(ReturnType);
        }

        private static GetRoleDTO ToDto(Role role)
        {
            return new GetRoleDTO
            {
                ClientId = role.ClientId,
                Id = role.Id,
                Name = role.Name
            };
        }
        private async Task<bool> HasChanged(Role role)
        {
            var lastest = await Exists(role.Id);
            return !(role.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Role> Exists(Guid id)
        {
            var current = await _db.Roles.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }
}
