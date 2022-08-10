using SSOService.Models;
using SSOService.Models.DTOs.Subscription;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Response<IEnumerable<GetSubscriptionDTO>>> Get(string name);
        Task<Response<GetSubscriptionDTO>> Get(Guid id);
        Task<Response<GetSubscriptionDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Task<Response<GetSubscriptionDTO>> Update(Guid id, UpdateSubscriptionDTO subscriptionDTO);
        Task<Response<GetSubscriptionDTO>> Create(CreateSubscriptionDTO subscriptionDTO);
    }
}
