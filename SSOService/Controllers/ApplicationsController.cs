using Microsoft.AspNetCore.Mvc;
using SSOService.Models.Domains;
using SSOService.Models.Enums;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.DTOs.Application;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedRequest]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationsController(IApplicationRepository applicationRepository) =>
            _applicationRepository = applicationRepository;

        // GET: api/Applications
        [HttpGet]
        public ActionResult<Response<IEnumerable<GetApplicationDTO>>> GetApplications(string name = null,
            ApplicationType? applicationType = null, Entity? serviceType = null)
            => Ok(_applicationRepository.Get(name, applicationType, serviceType));

        // GET: api/Applications/5
        [HttpGet("{id}")]
        public ActionResult<Response<Application>> GetApplication(Guid id)
            => Ok(_applicationRepository.Get(id));

        // PUT: api/Applications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateApplication(Guid id, UpdateApplicationDTO application)
            => Ok(await _applicationRepository.Update(id, application));

        // POST: api/Applications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GetApplicationDTO>> PostApplication(CreateApplicationDTO application)
            => Ok(await _applicationRepository.Create(application));

        // DELETE: api/Applications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(Guid id)
            => Ok(await _applicationRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetApplicationDTO>>> Activate(Guid id)
            => Ok(await _applicationRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetApplicationDTO>>> Deactivate(Guid id)
            => Ok(await _applicationRepository.ChangeState(id, true));
    }
}
