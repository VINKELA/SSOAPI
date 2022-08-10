using Microsoft.EntityFrameworkCore;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.ServiceType;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class ServiceTypeRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetServiceTypeDTO ReturnType = new();
        public ServiceTypeRepository(IUserRepository userRepository, SSODbContext db,
            IServiceResponse serviceResponse)
        {
            _userRepository = userRepository;
            _db = db;
            _response = serviceResponse;
        }
        public async Task<Response<GetServiceTypeDTO>> Create(CreateServiceTypeDTO serviceTypeDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var application = new ServiceType
            {
                Name = serviceTypeDTO.Name,
                ClientId = serviceTypeDTO.ClientId,
                CreatedBy = currentUser.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceTypeDTO>> Update(Guid id, UpdateServiceTypeDTO serviceTypeDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var currentServiceType = _db.ServiceTypes.FirstOrDefault(x => x.Id == id);
            var application = new ServiceType
            {
                Name = serviceTypeDTO.Name?.ToLower() ?? currentServiceType.Name,
                LastModifiedBy = currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentServiceType);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(currentServiceType))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceTypeDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            var currentUser = _userRepository.GetLoggedInUser();
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.ServiceType));
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
            _db.ServiceTypes.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(currentUser.Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceTypeDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.ServiceType));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetServiceTypeDTO>>> Get(string name)
        {
            var user = _userRepository.GetLoggedInUser();
            var list = await _db.ServiceTypes.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            return _response.SuccessResponse(list.Select(x => ToDto(x)));
        }

        private static GetServiceTypeDTO ToDto(ServiceType serviceType)
        {
            return new GetServiceTypeDTO
            {
                ClientId = serviceType.ClientId,
                Id = serviceType.Id,
                Name = serviceType.Name
            };
        }
        private async Task<bool> HasChanged(ServiceType serviceType)
        {
            var lastest = await Exists(serviceType.Id);
            return !(serviceType.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<ServiceType> Exists(Guid id)
        {
            var current = await _db.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }
}
