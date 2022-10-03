using Microsoft.AspNetCore.Mvc;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.DTOs.Permission;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedRequest]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionsController(IPermissionRepository permissionRepository)
            => _permissionRepository = permissionRepository;

        // GET: api/Permissions
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetPermissionDTO>>>> GetPermissions(string name = null)
            => Ok(await _permissionRepository.Get(name));

        // GET: api/Permissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetPermissionDTO>>> GetPermission(long id)
            => Ok(await _permissionRepository.Get(id));

        // PUT: api/Permissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetPermissionDTO>>> PutPermission(long id, UpdatePermissionDTO permission)
            => Ok(await _permissionRepository.Update(id, permission));

        // POST: api/Permissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetPermissionDTO>>> PostPermission(CreatePermissionDTO permission)
            => Ok(await _permissionRepository.Create(permission));

        // DELETE: api/Permissions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetPermissionDTO>>> DeletePermission(long id)
            => Ok(await _permissionRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetPermissionDTO>>> Activate(long id)
            => Ok(await _permissionRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetPermissionDTO>>> Deactivate(long id)
            => Ok(await _permissionRepository.ChangeState(id, true));
    }
}
