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
        Task<Response<GetClientDTO>> Update(long id, UpdateClientDTO client);
        Task<Response<GetClientDTO>> ChangeState(long id, bool deactivate = false, bool delete = false);
        Task<Response<GetClientDTO>> Get(long id);
        Task<Response<List<GetClientDTO>>> Get(string name, string contactPersonEmail, ClientType? clientType);
        Task<Response<GetClientDTO>> InitializeApp(CreateClientDTO client);

    }
}
