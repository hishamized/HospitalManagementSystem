using HMS.Application.DTO.PatientPortal;
using HMS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IPatientPortalRepository 
    {
        Task<GetPatientByIdentifierDto> GetPatientByIdentifierAsync(CancellationToken token, string identifier);
        Task<PatientLoginResultDto> LoginAsync(CancellationToken token, string identifier);
        Task<IEnumerable< GetPatientVisitsResponseDto>> GetPatientVisitsByIdAsync(CancellationToken token, long PatientId);
        Task<IEnumerable<GetPatientBillsDto>> GetPatientBillsAsync(long PatientId, long VisitId, CancellationToken cancellationToken);

    }
}
