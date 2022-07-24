using SSOService.Models;
using SSOService.Models.DTOs;
using SSOService.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IClientRepository
    {
        Task<Response<GetClientDTO>> Save(CreateClientDTO client);
        Task<Response<GetClientDTO>> Update(Guid id, UpdateClientDTO client);
        Task<Response<GetClientDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Response<GetClientDTO> Get(Guid id);
        Task<Response<List<GetClientDTO>>> Get(string name, string contactPersonEmail, ClientType? clientType);

    }
}
