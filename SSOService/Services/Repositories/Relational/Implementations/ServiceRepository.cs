using Microsoft.EntityFrameworkCore;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Service;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetServiceDTO ReturnType = new();
        public ServiceRepository(IUserRepository userRepository, SSODbContext db,
            IServiceResponse serviceResponse)
        {
            _userRepository = userRepository;
            _db = db;
            _response = serviceResponse;
        }
        public async Task<Response<GetServiceDTO>> Create(CreateServiceDTO serviceDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var application = new Service
            {
                Name = serviceDTO.Name,
                ClientId = serviceDTO.ClientId,
                CreatedBy = currentUser.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceDTO>> Update(Guid id, UpdateServiceDTO serviceDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var currentService = _db.Services.FirstOrDefault(x => x.Id == id);
            var application = new Service
            {
                Name = serviceDTO.Name?.ToLower() ?? currentService.Name,
                LastModifiedBy = currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentService);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(currentService))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            var currentUser = _userRepository.GetLoggedInUser();
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Service));
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
            _db.Services.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(currentUser.Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetServiceDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetServiceDTO>>> Get(string name)
        {
            var user = _userRepository.GetLoggedInUser();
            var list = await _db.Services.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            return _response.SuccessResponse(list.Select(x => ToDto(x)));
        }

        private static GetServiceDTO ToDto(Service service) => new()
        {
            ClientId = service.ClientId,
            Id = service.Id,
            Name = service.Name
        };
        private async Task<bool> HasChanged(Service service)
        {
            var lastest = await Exists(service.Id);
            return !(service.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Service> Exists(Guid id)
        {
            var current = await _db.Services.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }
}
