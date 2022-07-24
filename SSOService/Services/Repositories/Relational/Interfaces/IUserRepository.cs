using SSOService.Models;
using SSOService.Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public interface IUserRepository
    {
        Task<Response<GetUserDTO>> Save(CreateUserDTO user);
        Task<Response<GetUserDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Response<GetUserDTO> Get(Guid id);
        Task<Response<IEnumerable<GetUserDTO>>> Get(string name, string email,
   string phoneNumber, string client);
        Task<Response<GetUserDTO>> Update(Guid id, UpdateUserDTO user);

    }
}
