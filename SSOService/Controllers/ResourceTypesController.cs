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
        private readonly IResourceType _resourceTypeRepository;

        public ResourceTypesController(IResourceType resourceTypeRepository)
            => _resourceTypeRepository = resourceTypeRepository;

        // GET: api/resourceTypes
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetResourceTypeDTO>>>> GetresourceTypes(string name = null)
            => Ok(await _resourceTypeRepository.Get(name));

        // GET: api/resourceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetResourceTypeDTO>>> GetresourceType(Guid id)
            => Ok(await _resourceTypeRepository.Get(id));

        // PUT: api/resourceTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetResourceTypeDTO>>> PutresourceType(Guid id, UpdateResourceTypeDTO resourceType)
            => Ok(await _resourceTypeRepository.Update(id, resourceType));

        // POST: api/resourceTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetResourceTypeDTO>>> PostresourceType(CreateResourceTypeDTO resourceType)
            => Ok(await _resourceTypeRepository.Create(resourceType));

        // DELETE: api/resourceTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetResourceTypeDTO>>> DeleteresourceType(Guid id)
            => Ok(await _resourceTypeRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetResourceTypeDTO>>> Activate(Guid id)
            => Ok(await _resourceTypeRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetResourceTypeDTO>>> Deactivate(Guid id)
            => Ok(await _resourceTypeRepository.ChangeState(id, true));
    }
}
