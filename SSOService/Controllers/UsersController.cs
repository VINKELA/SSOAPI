﻿using Microsoft.AspNetCore.Mvc;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.User;
using SSOService.Services.Repositories.Relational.Interfaces;
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
        [AuthorizedRequest]

        public async Task<ActionResult<IEnumerable<GetUserDTO>>> Get(string name = null, string email = null,
   string phoneNumber = null, string client = null) => Ok(await _user.Get(name, email, phoneNumber, client));
        [HttpPatch("{id}")]
        public async Task<ActionResult<GetUserDTO>> Post(long id, [FromForm] UpdateUserDTO user)
            => Ok(await _user.Update(id, user));

        // GET: api/Users/5
        [HttpGet("{id}")]
        [AuthorizedRequest]
        public async Task<ActionResult<User>> GetUser(long id) => Ok(await _user.Get(id));
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult<Response<GetUserDTO>>> Activate(long id)
            => Ok(await _user.ChangeState(id));

        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult<Response<GetUserDTO>>> Deactivate(long id)
            => Ok(await _user.ChangeState(id, true));

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<GetUserDTO>>> Post([FromForm] CreateUserDTO user)
            => Ok(await _user.CreateAsync(user));

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [AuthorizedRequest]
        public async Task<ActionResult<Response<GetUserDTO>>> Delete(long id)
            => Ok(await _user.ChangeState(id, false, true));
    }
}
