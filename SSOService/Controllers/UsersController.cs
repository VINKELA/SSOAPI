using Microsoft.AspNetCore.Mvc;
using SSOMachine.Models.Domains;
using SSOService.Models;
using SSOService.Models.DTOs.User;
using SSOService.Services.Repositories.Relational.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _user;

        public UsersController(IUserRepository user)
        {
            _user = user;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDTO>>> Get(string name = null, string email = null,
   string phoneNumber = null, string client = null) => Ok(await _user.Get(name, email, phoneNumber, client));
        [HttpPut("{id}")]
        public async Task<ActionResult<GetUserDTO>> Post(Guid id, [FromForm] UpdateUserDTO user)
            => Ok(await _user.Update(id, user));

        // GET: api/Users/5
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(Guid id) => Ok(_user.Get(id));

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetUserDTO>>> Activate(Guid id)
            => Ok(await _user.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetUserDTO>>> Deactivate(Guid id)
            => Ok(await _user.ChangeState(id, true));


        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetUserDTO>>> Post([FromForm] CreateUserDTO user)
            => Ok(await _user.Save(user));

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetUserDTO>>> Delete(Guid id)
            => Ok(await _user.ChangeState(id, false, true));


    }
}
