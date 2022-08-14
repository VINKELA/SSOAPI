using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Subscription;
using SSOService.Models.DTOs.User;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOService.Subscriptions.Repositories.Relational.Implementations
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetSubscriptionDTO ReturnType = new();
        private readonly IResourceRepository _serviceRepository;
        private readonly IHttpContextAccessor _httpContext;

        public SubscriptionRepository(SSODbContext db,
            IServiceResponse subscriptionResponse, IResourceRepository serviceRepository, IHttpContextAccessor httpContext)
        {
            _db = db;
            _response = subscriptionResponse;
            _serviceRepository = serviceRepository;
            _httpContext = httpContext;
        }
        public async Task<Response<GetSubscriptionDTO>> Create(CreateSubscriptionDTO subscriptionDTO)
        {
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var application = new Subscription
            {
                Name = subscriptionDTO.Name,
                ClientId = subscriptionDTO.ClientId.GetValueOrDefault(),
                CreatedBy = currentUser.Email
            };
            await _db.AddAsync(application);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(application))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetSubscriptionDTO>> Update(Guid id, UpdateSubscriptionDTO subscriptionDTO)
        {
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var currentSubscription = _db.Subscriptions.FirstOrDefault(x => x.Id == id);
            var application = new Subscription
            {
                Name = subscriptionDTO.Name?.ToLower() ?? currentSubscription.Name,
                LastModifiedBy = currentUser.Email,
                Modified = DateTime.Now
            };
            await _db.AddAsync(currentSubscription);
            var status = await _db.SaveAndAuditChangesAsync(currentUser.Id) > 0;
            return status ? _response.SuccessResponse(ToDto(currentSubscription))
                : _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetSubscriptionDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            var currentUser = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Subscription));
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
            _db.Subscriptions.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(currentUser.Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetSubscriptionDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<GetSubscriptionDTO> GetSubscriptionById(Guid id)
        {
            var current = await Exists(id);
            if (current == null) return null;
            return ToDto(current);
        }

        public async Task<Response<IEnumerable<GetSubscriptionDTO>>> Get(string name)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var list = await _db.Subscriptions.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                list = list.Where(x => x.Name.ToUpper().Contains(name)).ToList();
            }
            return _response.SuccessResponse(list.Select(x => ToDto(x)));
        }
        public async Task<Response<GetSubscriptionDTO>> AddService(Guid serviceId, Guid subcriptionId)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];

            var service = await _serviceRepository.Get(serviceId);
            var subscription = await Exists(subcriptionId);

            if (service == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Resource));
            if (subscription == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Subscription));
            var newAuth = new SubscriptionResource
            {
                ResourceId = serviceId,
                SubscriptionId = subcriptionId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(subscription));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetSubscriptionDTO>> UpdateSubscriptionService(Guid serviceId, Guid subcriptionId, bool update)
        {
            var user = (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];
            var current = await _db.SubscriptionServices.FirstOrDefaultAsync(x => x.ResourceId == serviceId && x.SubscriptionId == subcriptionId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Subscription));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(current.SubscriptionId)));
            return _response.FailedResponse(ReturnType);
        }

        private static GetSubscriptionDTO ToDto(Subscription subscription)
        {
            return new GetSubscriptionDTO
            {
                ClientId = subscription.ClientId,
                Id = subscription.Id,
                Name = subscription.Name
            };
        }
        private async Task<bool> HasChanged(Subscription subscription)
        {
            var lastest = await Exists(subscription.Id);
            return !(subscription.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private async Task<Subscription> Exists(Guid id)
        {
            var current = await _db.Subscriptions.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }

    }
}
