using Microsoft.AspNetCore.Mvc;
using SSOService.Models;
using SSOService.Models.DTOs;
using SSOService.Models.Enums;
using SSOService.Models.Enums.Dictionary;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _client;

        public ClientsController(IClientRepository client)
            => _client = client;


        // POST: api/Clients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(type: typeof(Response<GetClientDTO>), statusCode: 200)]
        public async Task<IActionResult> PostClient([FromForm] CreateClientDTO client)
            => Ok(await _client.Save(client));
        [HttpGet("clientTypes")]
        public ActionResult<Response<List<List<EnumList>>>> GetClientTypes()
            => Ok(Response<List<EnumList>>.Success(EnumDictionary.GetList<ClientType>()));
        [HttpDelete("{id}")]
        [ProducesResponseType(type: typeof(Response<GetClientDTO>), statusCode: 200)]
        public async Task<IActionResult> DeleteClient(Guid id)
            => Ok(await _client.ChangeState(id, false, true));
        [HttpPatch("activate/{id}")]
        [ProducesResponseType(type: typeof(Response<GetClientDTO>), statusCode: 200)]
        public async Task<IActionResult> ActivateClient(Guid id)
            => Ok(await _client.ChangeState(id));
        [HttpPatch("deactivate/{id}")]
        [ProducesResponseType(type: typeof(Response<GetClientDTO>), statusCode: 200)]
        public async Task<IActionResult> DeactivateClient(Guid id)
            => Ok(await _client.ChangeState(id, true));
        [HttpGet("{id}")]
        [ProducesResponseType(type: typeof(Response<GetClientDTO>), statusCode: 200)]
        public IActionResult GetById(Guid id)
            => Ok(_client.Get(id));
        [HttpGet]
        [ProducesResponseType(type: typeof(Response<GetClientDTO>), statusCode: 200)]
        public async Task<IActionResult> Get(string name,
      string contactPersonEmail, ClientType? clientType)
        => Ok(await _client.Get(name, contactPersonEmail, clientType));
        [HttpPut("{id}")]
        [ProducesResponseType(type: typeof(Response<GetClientDTO>), statusCode: 200)]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateClientDTO client)
        => Ok(await _client.Update(id, client));









    }
}
