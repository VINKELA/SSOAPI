using Microsoft.AspNetCore.Mvc;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.Role;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedRequest]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RolesController(IRoleRepository roleRepository)
            => _roleRepository = roleRepository;

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetRoleDTO>>>> GetRoles(string name = null)
            => Ok(await _roleRepository.Get(name));

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetRoleDTO>>> GetRole(Guid id)
            => Ok(await _roleRepository.Get(id));

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetRoleDTO>>> PutRole(Guid id, UpdateRoleDTO role)
            => Ok(await _roleRepository.Update(id, role));

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetRoleDTO>>> PostRole(CreateRoleDTO role)
            => Ok(await _roleRepository.Create(role));

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetRoleDTO>>> DeleteRole(Guid id)
            => Ok(await _roleRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetRoleDTO>>> Activate(Guid id)
            => Ok(await _roleRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetRoleDTO>>> Deactivate(Guid id)
            => Ok(await _roleRepository.ChangeState(id, true));
    }
}
