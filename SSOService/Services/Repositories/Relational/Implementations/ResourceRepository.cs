using Microsoft.EntityFrameworkCore;
using SSOService.Helpers;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Service;
using SSOService.Models.DTOs.User;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetResourceDTO ReturnType = new();
        private readonly GetUserDTO _currentUser = RequestContext.GetCurrentUser;

        public ResourceRepository(SSODbContext db, IServiceResponse serviceResponse)
        {
            _db = db;
            _response = serviceResponse;
        }
        public async Task<Response<GetResourceDTO>> Create(CreateResourceDTO serviceDTO)
        {

            var application = new Resource
            {
                Name = serviceDTO.Name,
                ApplicationId = serviceDTO.ResourceTypeId,
                CreatedBy = _currentUser?.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveChangesAsync() > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<IEnumerable<GetResourceDTO>>> Create(List<CreateResourceDTO> resourceDTO)
        {

            var list = resourceDTO.Select(x => new Resource
            {
                Name = x.Name,
                ResourceTypeId = x.ResourceTypeId,
                ApplicationId = x.ApplicationId,
                CreatedBy = _currentUser?.Email
            });
            await _db.AddRangeAsync(list);
            await _db.SaveChangesAsync();
            return await Get(null);
        }

        public async Task<Response<GetResourceDTO>> Update(Guid id, UpdateResourceDTO serviceDTO)
        {

            var currentService = _db.Resources.FirstOrDefault(x => x.Id == id);
            var application = new Resource
            {
                Name = serviceDTO.Name?.ToLower() ?? currentService.Name,
                LastModifiedBy = _currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentService);
            var status = await _db.SaveChangesAsync() > 0;
            return status ? _response.SuccessResponse(ToDto(currentService))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetResourceDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);

            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Resource));
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
            _db.Resources.Update(current);
            var result = await _db.SaveChangesAsync();
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetResourceDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetResourceDTO>>> Get(string name)
        {

            var list = await _db.Resources.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            return _response.SuccessResponse(list.Select(x => ToDto(x)));
        }

        private static GetResourceDTO ToDto(Resource service) => new()
        {
            ClientId = service.ApplicationId,
            Id = service.Id,
            Name = service.Name
        };
        private async Task<bool> HasChanged(Resource service)
        {
            var lastest = await Exists(service.Id);
            return !(service.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Resource> Exists(Guid id)
        {
            var current = await _db.Resources.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }
}
