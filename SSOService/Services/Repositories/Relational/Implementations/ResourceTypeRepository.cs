using Microsoft.EntityFrameworkCore;
using SSOService.Helpers;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.ServiceType;
using SSOService.Models.DTOs.User;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class ResourceTypeRepository : IResourceType
    {
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetServiceTypeDTO ReturnType = new();
        private readonly GetUserDTO _currentUser = RequestContext.GetCurrentUser;

        public ResourceTypeRepository(SSODbContext db, IServiceResponse serviceResponse)
        {
            _db = db;
            _response = serviceResponse;
        }
        public async Task<Response<GetServiceTypeDTO>> Create(CreateServiceTypeDTO serviceTypeDTO)
        {

            var application = new ResourceType
            {
                Name = serviceTypeDTO.Name,
                ClientId = serviceTypeDTO.ClientId,
                CreatedBy = _currentUser.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveAndAuditChangesAsync(_currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceTypeDTO>> Update(Guid id, UpdateServiceTypeDTO serviceTypeDTO)
        {

            var currentServiceType = _db.ResourceTypes.FirstOrDefault(x => x.Id == id);
            var application = new ResourceType
            {
                Name = serviceTypeDTO.Name?.ToLower() ?? currentServiceType.Name,
                LastModifiedBy = _currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentServiceType);
            var status = await _db.SaveAndAuditChangesAsync(_currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(currentServiceType))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceTypeDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);

            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Resource));
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
            _db.ResourceTypes.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(_currentUser.Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceTypeDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.ResourceType));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetServiceTypeDTO>>> Get(string name)
        {

            var list = await _db.ResourceTypes.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            return _response.SuccessResponse(list.Select(x => ToDto(x)));
        }

        private static GetServiceTypeDTO ToDto(ResourceType serviceType)
        {
            return new GetServiceTypeDTO
            {
                ClientId = serviceType.ClientId,
                Id = serviceType.Id,
                Name = serviceType.Name
            };
        }
        private async Task<bool> HasChanged(ResourceType serviceType)
        {
            var lastest = await Exists(serviceType.Id);
            return !(serviceType.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<ResourceType> Exists(Guid id)
        {
            var current = await _db.ResourceTypes.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }
}
