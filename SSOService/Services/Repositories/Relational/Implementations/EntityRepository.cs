using Microsoft.EntityFrameworkCore;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Entity;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class EntityRepository : IEntityRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetEntityDTO ReturnType = new();
        public EntityRepository(IUserRepository userRepository, SSODbContext db,
            IServiceResponse serviceResponse)
        {
            _userRepository = userRepository;
            _db = db;
            _response = serviceResponse;
        }
        public async Task<Response<GetEntityDTO>> Create(CreateEntityDTO entityDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var application = new Entity
            {
                Name = entityDTO.Name,
                ApplicationId = entityDTO.ApplicationId,
                CreatedBy = currentUser.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetEntityDTO>> Update(Guid id, UpdateEntityDTO entityDTO)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var currentEntity = _db.Entities.FirstOrDefault(x => x.Id == id);
            var application = new Entity
            {
                Name = entityDTO.Name?.ToLower() ?? currentEntity.Name,
                LastModifiedBy = currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentEntity);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(currentEntity))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetEntityDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            var currentUser = _userRepository.GetLoggedInUser();
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Entity));
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
            _db.Entities.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(currentUser.Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetEntityDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<IEnumerable<GetEntityDTO>>> Get(string name)
        {
            var user = _userRepository.GetLoggedInUser();
            var list = await _db.Entities.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            return _response.SuccessResponse(list.Select(x => ToDto(x)).AsEnumerable());
        }

        private static GetEntityDTO ToDto(Entity entity)
        {
            return new GetEntityDTO
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
        private async Task<bool> HasChanged(Entity application)
        {
            var lastest = await Exists(application.Id);
            return !(application.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Entity> Exists(Guid id)
        {
            var current = await _db.Entities.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }

}
