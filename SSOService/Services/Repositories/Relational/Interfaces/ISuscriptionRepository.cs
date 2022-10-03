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
        Task<Response<GetSubscriptionDTO>> Get(long id);
        Task<Response<GetSubscriptionDTO>> ChangeState(long id, bool deactivate = false, bool delete = false);
        Task<Response<GetSubscriptionDTO>> Update(long id, UpdateSubscriptionDTO subscriptionDTO);
        Task<Response<GetSubscriptionDTO>> Create(CreateSubscriptionDTO subscriptionDTO);
        Task<GetSubscriptionDTO> GetSubscriptionById(long id);

    }
}
