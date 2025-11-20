using HMS.Application.Commands.DoctorPortal;
using HMS.Application.DTO.DoctorPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IDoctorPortalRepository
    {
        Task<DoctorLoginResultDto> LoginAsync(CancellationToken cancellationToken, string identifier);
        Task<IEnumerable<FetchPatientsByDoctorDto>> FetchPatientsByDoctorAsync(FetchPatientsByDoctorCommand request, CancellationToken cancellationToken);
    }
}
