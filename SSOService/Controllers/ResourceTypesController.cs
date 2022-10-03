using Microsoft.AspNetCore.Mvc;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.DTOs.ReSourceType;
using SSOService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOresource.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedRequest]
    public class ResourceTypesController : ControllerBase
    {
        private readonly IServiceType _resourceTypeRepository;

        public ResourceTypesController(IServiceType resourceTypeRepository)
            => _resourceTypeRepository = resourceTypeRepository;

        // GET: api/resourceTypes
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetServiceTypeDTO>>>> GetresourceTypes(string name = null)
            => Ok(await _resourceTypeRepository.Get(name));

        // GET: api/resourceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> GetresourceType(long id)
            => Ok(await _resourceTypeRepository.Get(id));

        // PUT: api/resourceTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> PutresourceType(long id, UpdateServiceTypeDTO resourceType)
            => Ok(await _resourceTypeRepository.Update(id, resourceType));

        // POST: api/resourceTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> PostresourceType(CreateServiceTypeDTO resourceType)
            => Ok(await _resourceTypeRepository.Create(resourceType));

        // DELETE: api/resourceTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> DeleteresourceType(long id)
            => Ok(await _resourceTypeRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> Activate(long id)
            => Ok(await _resourceTypeRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetServiceTypeDTO>>> Deactivate(long id)
            => Ok(await _resourceTypeRepository.ChangeState(id, true));
    }
}
