using HMS.Application.Commands.DoctorPortal;
using HMS.Application.Commands.LabTest;
using HMS.Application.DTO.DoctorPortal;
using HMS.Application.Queries.DoctorPortal;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IDoctorPortalRepository
    {
        Task<DoctorLoginResultDto> LoginAsync(CancellationToken cancellationToken, string identifier);
        Task<IEnumerable<FetchPatientsByDoctorDto>> FetchPatientsByDoctorAsync(FetchPatientsByDoctorCommand request, CancellationToken cancellationToken);
        Task<ViewPatientDto> ViewPatientAsync(ViewPatientQuery request, CancellationToken cancellationToken);
        Task<IEnumerable<FetchValidTestsDto>> FetchValidTestsAsync(FetchValidTestsQuery request, CancellationToken cancellationToken);
        Task<int> CreateLabRequestAsync(CancellationToken cancellationToken, LabRequest labRequest, DataTable itemsTable);
        Task<IEnumerable<FetchLabRequestsWithItemsByPatientDto>> FetchLabRequestsWithItemsByPatientAsync(FetchLabRequestsWithItemsByPatientCommand request, CancellationToken cancellationToken);
        Task<long> UpdateLabRequestItemStatusAsync(UpdateLabRequestItemStatusCommand request, CancellationToken cancellationToken);

    }
}
