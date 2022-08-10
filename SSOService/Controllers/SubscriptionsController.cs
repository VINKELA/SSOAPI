using Microsoft.AspNetCore.Mvc;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.DTOs.Subscription;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedRequest]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionsController(ISubscriptionRepository subscriptionRepository)
            => _subscriptionRepository = subscriptionRepository;

        // GET: api/Subscriptions
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetSubscriptionDTO>>>> GetSubscriptions(string name = null)
            => Ok(await _subscriptionRepository.Get(name));

        // GET: api/Subscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetSubscriptionDTO>>> GetSubscription(Guid id)
            => Ok(await _subscriptionRepository.Get(id));

        // PUT: api/Subscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetSubscriptionDTO>>> PutSubscription(Guid id, UpdateSubscriptionDTO subscription)
            => Ok(await _subscriptionRepository.Update(id, subscription));

        // POST: api/Subscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetSubscriptionDTO>>> PostSubscription(CreateSubscriptionDTO subscription)
            => Ok(await _subscriptionRepository.Create(subscription));

        // DELETE: api/Subscriptions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetSubscriptionDTO>>> DeleteSubscription(Guid id)
            => Ok(await _subscriptionRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetSubscriptionDTO>>> Activate(Guid id)
            => Ok(await _subscriptionRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetSubscriptionDTO>>> Deactivate(Guid id)
            => Ok(await _subscriptionRepository.ChangeState(id, true));



    }

}
