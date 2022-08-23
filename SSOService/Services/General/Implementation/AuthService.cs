using Microsoft.Extensions.Logging;
using SSOService.Helpers;
using SSOService.Models;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.Auth;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System.Threading.Tasks;

namespace SSOService.Services.General.Implementation
{
    public class AuthService : IAuth
    {
        private const string FailedLoginMessage = "Incorrect Credentials";
        private readonly IUserRepository _userRepository;
        private readonly IToken _token;
        private readonly IServiceResponse _serviceResponse;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ILogger<TokenDTO> _logger;


        public AuthService(IUserRepository userRepository, IToken token, IServiceResponse serviceResponse,
            IApplicationRepository applicationRepository, ILogger<TokenDTO> logger)
        {
            _userRepository = userRepository;
            _token = token;
            _serviceResponse = serviceResponse;
            _applicationRepository = applicationRepository;
            _logger = logger;

        }
        public async Task<Response<TokenDTO>> Login(LoginDTO user)
        {
            _logger.LogInformation("user login");
            var data = await _userRepository.GetUserByEmailOrUsername(user.EmailORUserName);
            if (data == null) return _serviceResponse.FailedResponse<TokenDTO>(null, FailedLoginMessage);
            var checkCredentials = HashEngine.VerifyHash(user.Password, data.PasswordHash);
            if (!checkCredentials) return _serviceResponse.FailedResponse<TokenDTO>(null, FailedLoginMessage);
            var token = await _token.BuildToken(data);
            return _serviceResponse.SuccessResponse(token);
        }
        public async Task<Response<TokenDTO>> Login(AppLoginDTO app)
        {
            var data = await _applicationRepository.Get(app);
            if (data.Data == null) return _serviceResponse.FailedResponse<TokenDTO>(null, FailedLoginMessage);
            var token = await _token.BuildToken(data.Data);
            return _serviceResponse.SuccessResponse(token);
        }

        //login a user
        //get clients, clientsuscription and services
        //get user permissions, suscription,roles, roleclaim, devices
    }
}
