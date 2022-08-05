using Microsoft.AspNetCore.Mvc;
using SSOService.Models;
using SSOService.Models.DTOs.Audit;
using SSOService.Models.DTOs.Auth;
using SSOService.Services.General.Interfaces;
using System.Threading.Tasks;

namespace SSOService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuth _auth;
        public AuthController(IAuth authService)
        {
            _auth = authService;
        }
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Login")]
        [ProducesResponseType(type: typeof(Response<TokenDTO>), statusCode: 200)]
        public async Task<IActionResult> PostClient([FromBody] LoginDTO user)
            => Ok(await _auth.Login(user));

    }
}
