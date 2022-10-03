using Microsoft.EntityFrameworkCore;
using SSOService.Extensions;
using SSOService.Helpers;
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
        private readonly GetApplicationDTO ReturnType = new();
        private readonly GetApplicationAuthentificationDTO ApplicationAuthorizationReturnType = new();
        private readonly GetUserDTO _currentUser = RequestContext.GetCurrentUser;

        public ApplicationRepository(SSODbContext db,
            IServiceResponse serviceResponse, IResourceRepository resourceRepository, IPermissionRepository permissionRepository)
        {
            _db = db;
            _response = serviceResponse;
            _resourceRepository = resourceRepository;
            _permissionRepository = permissionRepository;
        }
        public async Task<Response<GetApplicationDTO>> Create(CreateApplicationDTO applicationDTO)
        {

            var application = new Application
            {
                Name = applicationDTO.Name,
                ApplicationType = applicationDTO.ApplicationType,
                URL = applicationDTO.URL,
                ClientId = applicationDTO.ClientId,
                CreatedBy = _currentUser?.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveChangesAsync() > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetApplicationDTO>> Update(long id, UpdateApplicationDTO applicationDTO)
        {

            var currentApplication = await _db.Applications.FirstOrDefaultAsync(x => x.ApplicationId == id);
            var application = new Application
            {
                Name = applicationDTO.Name?.ToLower() ?? currentApplication.Name,
                ApplicationType = applicationDTO.ApplicationType ?? currentApplication.ApplicationType,
                URL = applicationDTO.URL?.ToLower() ?? currentApplication.URL,
                LastModifiedBy = _currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentApplication);
            var status = await _db.SaveChangesAsync() > 0;
            return status ? _response.SuccessResponse(ToDto(currentApplication))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetApplicationDTO>> ChangeState(long id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);

            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            if (deactivate) current.IsActive = !deactivate;
            else if (delete)
            {
                current.IsActive = !delete;
                current.IsDeleted = delete;
            }
            else current.IsActive = true;
            var hasChanged = await HasChanged(current);
            if (hasChanged)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, current.ApplicationId));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Applications.Update(current);
            var result = await _db.SaveChangesAsync();
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetApplicationDTO>> Get(long id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<GetApplicationDTO>> Get(AppLoginDTO app)
        {
            var current = await _db.ApplicationAuthentications.FirstOrDefaultAsync(x => x.Code == app.ClientCode && x.ClientSecret == app.ClientSecret);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            return await Get(current.ClientApplicationId);
        }

        public async Task<Response<IEnumerable<GetApplicationDTO>>> Get(string name, ApplicationType? applicationType,
            Entity? serviceType)
        {

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
        public async Task<Response<GetApplicationDTO>> UpdatePermission(long applicationId, long permissionId, bool update)
        {

            var current = await Exists(applicationId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            var permission = await _permissionRepository.Get(permissionId);
            if (permission.Data == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Permission));
            var applicationPermission = await _db.ApplicationPermissions.FirstOrDefaultAsync(x => x.PermissionId == permissionId &&
                                           x.ApplicationId == applicationId);
            if (applicationPermission == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.ApplicationPermission));
            applicationPermission.IsActive = update ? !applicationPermission.IsActive : applicationPermission.IsActive;
            applicationPermission.IsDeleted = !update ? !applicationPermission.IsDeleted : applicationPermission.IsDeleted;
            _db.Update(applicationPermission);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return await Get(applicationId);
            return _response.FailedResponse(ReturnType);
        }

        public async Task<Response<GetApplicationDTO>> AddPermissionToApplication(long applicationId, long permissionId)
        {


            var application = await Exists(applicationId);
            if (application == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            var permission = await _permissionRepository.Get(permissionId);
            if (permission.Data == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            await _db.AddAsync(new ApplicationPermission
            {
                ApplicationId = applicationId,
                PermissionId = permissionId
            }
                );
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return await Get(applicationId);
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetApplicationDTO>> RemoveResourceToApplication(long applicationId, long resourceId)
        {


            var application = await Exists(applicationId);
            if (application == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            var resource = await _resourceRepository.Get(resourceId);
            if (resource.Data == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Resource));
            await _db.AddAsync(new ResourceEndpoint
            {
                ResourceId = resourceId
            }
                );
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return await Get(applicationId);
            return _response.FailedResponse(ReturnType);
        }

        public async Task<Response<GetApplicationDTO>> UpdateResource(long applicationId, long serverId, bool update)
        {

            var current = await Exists(applicationId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            var resource = await _resourceRepository.Get(serverId);
            if (resource.Data == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Resource));
            var applicationResource = await _db.ResourceEndpoints.FirstOrDefaultAsync(x => x.ResourceId == serverId);
            if (applicationResource == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.ApplicationResource));
            applicationResource.IsActive = update ? !applicationResource.IsActive : applicationResource.IsActive;
            applicationResource.IsDeleted = !update ? !applicationResource.IsDeleted : applicationResource.IsDeleted;
            _db.Update(applicationResource);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return await Get(applicationId);
            return _response.FailedResponse(ReturnType);
        }

        public async Task<Response<GetApplicationAuthentificationDTO>> AddApplication(long applicationId, long serverId)
        {


            var application = await Exists(applicationId);
            var server = await Exists(serverId);

            if (application == null)
                return _response.FailedResponse(ApplicationAuthorizationReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            if (server == null)
                return _response.FailedResponse(ApplicationAuthorizationReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            var newAuth = new ApplicationAuthentication
            {
                ClientApplicationId = applicationId,
                ServerApplicationId = serverId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(await ToDto(newAuth.Code));
            return _response.FailedResponse(ApplicationAuthorizationReturnType);
        }
        public async Task<Response<GetApplicationAuthentificationDTO>> UpdateApplicationAuthentification(long applicationId, long serverId, bool update)
        {

            var current = await _db.ApplicationAuthentications.FirstOrDefaultAsync(x => x.ServerApplicationId == serverId && x.ClientApplicationId == applicationId);
            if (current == null)
                return _response.FailedResponse(ApplicationAuthorizationReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Application));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(await ToDto(current.Code));
            return _response.FailedResponse(ApplicationAuthorizationReturnType);
        }
        private static GetApplicationDTO ToDto(Application application)
        {
            return new GetApplicationDTO
            {
                ApplicationType = application.ApplicationType.DisplayName(),
                ClientId = application.ClientId,
                Id = application.ApplicationId,
                Name = application.Name,
                URL = application.URL
            };
        }
        private async Task<GetApplicationAuthentificationDTO> ToDto(string code)
        {
            var current = await _db.ApplicationAuthentications.FirstOrDefaultAsync(x => x.Code == code);
            var application = await Exists(current.ClientApplicationId);
            var server = await Exists(current.ServerApplicationId);

            return new GetApplicationAuthentificationDTO
            {
                Client = application.Name,
                ClientApplicationId = application.ApplicationId,
                Server = server.Name,
                ServerApplicationId = server.ApplicationId,
                ClientCode = server.Code,
                ClientSecret = current.ClientSecret

            };
        }
        private async Task<bool> HasChanged(Application application)
        {
            var lastest = await Exists(application.ApplicationId);
            return !(application.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Application> Exists(long id)
        {
            var current = await _db.Applications.FirstOrDefaultAsync(x => x.ApplicationId == id && !x.IsDeleted);
            return current;
        }

    }
}
