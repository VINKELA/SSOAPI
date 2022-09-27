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
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceRepository _serviceRepository;

        public ResourcesController(IResourceRepository serviceRepository)
            => _serviceRepository = serviceRepository;

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetResourceDTO>>>> GetServices(string name = null)
            => Ok(await _serviceRepository.Get(name));

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetResourceDTO>>> GetService(Guid id)
            => Ok(await _serviceRepository.Get(id));

        // PUT: api/Services/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetResourceDTO>>> PutService(Guid id, UpdateResourceDTO service)
            => Ok(await _serviceRepository.Update(id, service));

        // POST: api/Services
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetResourceDTO>>> PostService(CreateResourceDTO service)
            => Ok(await _serviceRepository.Create(service));

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetResourceDTO>>> DeleteService(Guid id)
            => Ok(await _serviceRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetResourceDTO>>> Activate(Guid id)
            => Ok(await _serviceRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetResourceDTO>>> Deactivate(Guid id)
            => Ok(await _serviceRepository.ChangeState(id, true));
    }
}
