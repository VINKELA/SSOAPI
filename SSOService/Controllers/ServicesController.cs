using Microsoft.AspNetCore.Mvc;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.DTOs.Service;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedRequest]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;

        public ServicesController(IServiceRepository serviceRepository)
            => _serviceRepository = serviceRepository;

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetServiceDTO>>>> GetServices(string name = null)
            => Ok(await _serviceRepository.Get(name));

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetServiceDTO>>> GetService(Guid id)
            => Ok(await _serviceRepository.Get(id));

        // PUT: api/Services/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetServiceDTO>>> PutService(Guid id, UpdateServiceDTO service)
            => Ok(await _serviceRepository.Update(id, service));

        // POST: api/Services
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetServiceDTO>>> PostService(CreateServiceDTO service)
            => Ok(await _serviceRepository.Create(service));

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetServiceDTO>>> DeleteService(Guid id)
            => Ok(await _serviceRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetServiceDTO>>> Activate(Guid id)
            => Ok(await _serviceRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetServiceDTO>>> Deactivate(Guid id)
            => Ok(await _serviceRepository.ChangeState(id, true));



    }
}
