using Microsoft.AspNetCore.Mvc;
using SSOService.Models;
using SSOService.Models.DTOs.Entity;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        private readonly IEntityRepository _entityRepository;

        public EntitiesController(IEntityRepository entityRepository)
            => _entityRepository = entityRepository;

        // GET: api/Entities
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<GetEntityDTO>>>> GetEntities(string name = null)
            => Ok(await _entityRepository.Get(name));

        // GET: api/Entities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<GetEntityDTO>>> GetEntity(Guid id)
            => Ok(await _entityRepository.Get(id));

        // PUT: api/Entities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<GetEntityDTO>>> PutEntity(Guid id, UpdateEntityDTO entity)
            => Ok(await _entityRepository.Update(id, entity));

        // POST: api/Entities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetEntityDTO>>> PostEntity(CreateEntityDTO entity)
            => Ok(await _entityRepository.Create(entity));

        // DELETE: api/Entities/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetEntityDTO>>> DeleteEntity(Guid id)
            => Ok(await _entityRepository.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetEntityDTO>>> Activate(Guid id)
            => Ok(await _entityRepository.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetEntityDTO>>> Deactivate(Guid id)
            => Ok(await _entityRepository.ChangeState(id, true));



    }
}
