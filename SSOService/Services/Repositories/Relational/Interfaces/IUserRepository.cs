using SSOService.Models;
using SSOService.Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IUserRepository
    {
        Task<Response<GetUserDTO>> CreateAsync(CreateUserDTO user);
        Task<Response<GetUserDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Task<Response<GetUserDTO>> Get(Guid id);
        Task<Response<IEnumerable<GetUserDTO>>> Get(string name, string email,
            string phoneNumber, string client);
        Task<Response<GetUserDTO>> Update(Guid id, UpdateUserDTO user);
        Task<Response<GetUserDTO>> Get(string emailOrUsername);
        Task<GetUserDTO> GetUserByEmailOrUsername(string emailOrUsername);
        GetUserDTO GetLoggedInUser();
        Task<Response<GetUserDTO>> AddRole(Guid roleId, Guid userId);
    }
}
