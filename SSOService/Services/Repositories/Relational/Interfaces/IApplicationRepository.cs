using SSOMachine.Models.Enums;
using SSOService.Models;
using SSOService.Models.DTOs.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Response<IEnumerable<GetApplicationDTO>>> Get(string name, ApplicationType? applicationType,
        ServiceType? serviceType);
        Task<Response<GetApplicationDTO>> Get(Guid id);
        Task<Response<GetApplicationDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false);
        Task<Response<GetApplicationDTO>> Update(Guid id, UpdateApplicationDTO applicationDTO);
        Task<Response<GetApplicationDTO>> Create(CreateApplicationDTO applicationDTO);

    }
}
