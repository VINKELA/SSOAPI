using Microsoft.EntityFrameworkCore;
using SSOService.Models.Domains;
using SSOService.Models.Enums;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.DTOs.Application;
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
        private readonly IUserRepository _userRepository;
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetApplicationDTO ReturnType = new();
        public ApplicationRepository(IUserRepository userRepository, SSODbContext db,
            IServiceResponse serviceResponse)
        {
            _userRepository = userRepository;
            _db = db;
            _response = serviceResponse;
        }
        public async Task<Response<GetApplicationDTO>> Create(CreateApplicationDTO applicationDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var application = new Application
            {
                Name = applicationDTO.Name,
                ApplicationType = applicationDTO.ApplicationType,
                URL = applicationDTO.URL,
                ServiceTypeId = applicationDTO.ServiceTypeId,
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
            var currentUser = _userRepository.GetLoggedInUser();
            var currentApplication = await _db.Applications.FirstOrDefaultAsync(x => x.Id == id);
            var application = new Application
            {
                Name = applicationDTO.Name?.ToLower() ?? currentApplication.Name,
                ApplicationType = applicationDTO.ApplicationType ?? currentApplication.ApplicationType,
                URL = applicationDTO.URL?.ToLower() ?? currentApplication.URL,
                ServiceTypeId = applicationDTO.ServiceTypeId ?? currentApplication.ServiceTypeId,
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
            var currentUser = _userRepository.GetLoggedInUser();
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
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetApplicationDTO>>> Get(string name, ApplicationType? applicationType,
            Entity? serviceType)
        {
            var user = _userRepository.GetLoggedInUser();
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
