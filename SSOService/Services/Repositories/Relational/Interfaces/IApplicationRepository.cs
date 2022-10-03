using SSOService.Models;
using SSOService.Models.DTOs.Application;
using SSOService.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Response<IEnumerable<GetApplicationDTO>>> Get(string name, ApplicationType? applicationType,
        Entity? serviceType);
        Task<Response<GetApplicationDTO>> Get(long id);
        Task<Response<GetApplicationDTO>> ChangeState(long id, bool deactivate = false, bool delete = false);
        Task<Response<GetApplicationDTO>> Update(long id, UpdateApplicationDTO applicationDTO);
        Task<Response<GetApplicationDTO>> Create(CreateApplicationDTO applicationDTO);
        Task<Response<GetApplicationDTO>> Get(AppLoginDTO app);


    }
}
