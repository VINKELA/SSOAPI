using Microsoft.AspNetCore.Mvc;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.ServiceType;
using SSOService.Services.Interfaces;
using SSOService.Services.Repositories.Relational.Implementations;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedRequest]
    public class ServiceTypesController : ControllerBase
    {
        private readonly IServiceTypeRepository _serviceTypeRepository;

        public ServiceTypesController(IServiceTypeRepository serviceTypeRepository)
            => _serviceTypeRepository = serviceTypeRepository;

        // GET: api/ServiceTypes
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetServiceTypeDTO>>>> GetServiceTypes(string name = null)
            => Ok(await _serviceTypeRepository.Get(name));

        // GET: api/ServiceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> GetServiceType(Guid id)
            => Ok(await _serviceTypeRepository.Get(id));

        // PUT: api/ServiceTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> PutServiceType(Guid id, UpdateServiceTypeDTO serviceType)
            => Ok(await _serviceTypeRepository.Update(id, serviceType));

        // POST: api/ServiceTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> PostServiceType(CreateServiceTypeDTO serviceType)
            => Ok(await _serviceTypeRepository.Create(serviceType));

        // DELETE: api/ServiceTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> DeleteServiceType(Guid id)
            => Ok(await _serviceTypeRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> Activate(Guid id)
            => Ok(await _serviceTypeRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> Deactivate(Guid id)
            => Ok(await _serviceTypeRepository.ChangeState(id, true));



    }
}
