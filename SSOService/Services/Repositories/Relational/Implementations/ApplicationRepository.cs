using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.User;
using SSOService.Models.Enums;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly IResourceRepository _resourceRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly GetApplicationDTO ReturnType = new();
        private readonly GetApplicationAuthentificationDTO ApplicationAuthorizationReturnType = new();

        public ApplicationRepository(SSODbContext db, IHttpContextAccessor httpContext,
            IServiceResponse serviceResponse, IResourceRepository resourceRepository, IPermissionRepository permissionRepository)
        {
            _db = db;
            _response = serviceResponse;
            _resourceRepository = resourceRepository;
            _permissionRepository = permissionRepository;
            _httpContext = httpContext;
        }
        public async Task<Response<GetApplicationDTO>> Create(CreateApplicationDTO applicationDTO)
        {
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var application = new Application
            {
                Name = applicationDTO.Name,
                ApplicationType = applicationDTO.ApplicationType,
                URL = applicationDTO.URL,
                ClientId = applicationDTO.ClientId,
                CreatedBy = currentUser.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetApplicationDTO>> Update(Guid id, UpdateApplicationDTO applicationDTO)
        {
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var currentApplication = await _db.Applications.FirstOrDefaultAsync(x => x.Id == id);
            var application = new Application
            {
                Name = applicationDTO.Name?.ToLower() ?? currentApplication.Name,
                ApplicationType = applicationDTO.ApplicationType ?? currentApplication.ApplicationType,
                URL = applicationDTO.URL?.ToLower() ?? currentApplication.URL,
                LastModifiedBy = currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentApplication);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(currentApplication))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetApplicationDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
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
            _db.Applications.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(currentUser.Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetApplicationDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetApplicationDTO>>> Get(string name, ApplicationType? applicationType,
            Entity? serviceType)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var list = await _db.Applications.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            if (applicationType != null)
                list = list.Where(x => x.ApplicationType == x.ApplicationType).ToList();
            return _response.SuccessResponse(list.Select(x => ToDto(x)));
        }
        public async Task<Response<GetApplicationDTO>> UpdatePermission(Guid applicationId, Guid permissionId, bool update)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var current = await Exists(applicationId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            var permission = await _permissionRepository.Get(permissionId);
            if (permission.Data == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Permission));
            var applicationPermission = await _db.ApplicationPermissions.FirstOrDefaultAsync(x => x.PermissionId == permissionId &&
                                           x.ApplicationId == applicationId);
            if (applicationPermission == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.ApplicationPermission));
            applicationPermission.IsActive = update ? !applicationPermission.IsActive : applicationPermission.IsActive;
            applicationPermission.IsDeleted = !update ? !applicationPermission.IsDeleted : applicationPermission.IsDeleted;
            _db.Update(applicationPermission);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return await Get(applicationId);
            return _response.FailedResponse(ReturnType);
        }

        public async Task<Response<GetApplicationDTO>> AddPermissionToApplication(Guid applicationId, Guid permissionId)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];

            var application = await Exists(applicationId);
            if (application == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            var permission = await _permissionRepository.Get(permissionId);
            if (permission.Data == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            await _db.AddAsync(new ApplicationPermission
            {
                ApplicationId = applicationId,
                PermissionId = permissionId
            }
                );
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return await Get(applicationId);
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetApplicationDTO>> RemoveResourceToApplication(Guid applicationId, Guid resourceId)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];

            var application = await Exists(applicationId);
            if (application == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            var resource = await _resourceRepository.Get(resourceId);
            if (resource.Data == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Resource));
            await _db.AddAsync(new ApplicationResource
            {
                ApplicationId = applicationId,
                ResourceId = resourceId
            }
                );
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return await Get(applicationId);
            return _response.FailedResponse(ReturnType);
        }

        public async Task<Response<GetApplicationDTO>> UpdateResource(Guid applicationId, Guid serverId, bool update)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var current = await Exists(applicationId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            var resource = await _resourceRepository.Get(serverId);
            if (resource.Data == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Resource));
            var applicationResource = await _db.ApplicationResources.FirstOrDefaultAsync(x => x.ResourceId == serverId &&
                                           x.ApplicationId == applicationId);
            if (applicationResource == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.ApplicationResource));
            applicationResource.IsActive = update ? !applicationResource.IsActive : applicationResource.IsActive;
            applicationResource.IsDeleted = !update ? !applicationResource.IsDeleted : applicationResource.IsDeleted;
            _db.Update(applicationResource);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return await Get(applicationId);
            return _response.FailedResponse(ReturnType);
        }

        public async Task<Response<GetApplicationAuthentificationDTO>> AddApplication(Guid applicationId, Guid serverId)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];

            var application = await Exists(applicationId);
            var server = await Exists(serverId);

            if (application == null)
                return _response.FailedResponse(ApplicationAuthorizationReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            if (server == null)
                return _response.FailedResponse(ApplicationAuthorizationReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            var newAuth = new ApplicationAuthentification
            {
                ClientApplicationId = applicationId,
                ServerApplicationId = serverId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(await ToDto(newAuth.Code));
            return _response.FailedResponse(ApplicationAuthorizationReturnType);
        }
        public async Task<Response<GetApplicationAuthentificationDTO>> UpdateApplicationAuthentification(Guid applicationId, Guid serverId, bool update)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var current = await _db.ApplicationAuthentifications.FirstOrDefaultAsync(x => x.ServerApplicationId == serverId && x.ClientApplicationId == applicationId);
            if (current == null)
                return _response.FailedResponse(ApplicationAuthorizationReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Application));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(await ToDto(current.Code));
            return _response.FailedResponse(ApplicationAuthorizationReturnType);
        }
        private static GetApplicationDTO ToDto(Application application)
        {
            return new GetApplicationDTO
            {
                ApplicationType = application.ApplicationType.DisplayName(),
                ClientId = application.ClientId,
                Id = application.Id,
                Name = application.Name,
                URL = application.URL
            };
        }
        private async Task<GetApplicationAuthentificationDTO> ToDto(string code)
        {
            var current = await _db.ApplicationAuthentifications.FirstOrDefaultAsync(x => x.Code == code);
            var application = await Exists(current.ClientApplicationId);
            var server = await Exists(current.ServerApplicationId);

            return new GetApplicationAuthentificationDTO
            {
                Client = application.Name,
                ClientApplicationId = application.Id,
                Server = server.Name,
                ServerApplicationId = server.Id,
                ClientCode = server.Code,
                ClientSecret = current.ClientSecret

            };
        }
        private async Task<bool> HasChanged(Application application)
        {
            var lastest = await Exists(application.Id);
            return !(application.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Application> Exists(Guid id)
        {
            var current = await _db.Applications.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }
}
