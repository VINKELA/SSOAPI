using SSOService.Models;
using SSOService.Models.DTOs.User;
using System;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public interface IUserRepository
    {
        Task<Response<GetUserDTO>> Save(CreateUserDTO user);
        Task<Response<GetUserDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);

    }
}
