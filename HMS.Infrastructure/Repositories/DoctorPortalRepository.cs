using HMS.Application.Commands.DoctorPortal;
using HMS.Application.DTO.DoctorPortal;
using HMS.Application.DTO.PatientPortal;
using HMS.Application.Interfaces;
using HMS.Application.Queries.DoctorPortal;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using HMS.Domain.Logger.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class DoctorPortalRepository : IDoctorPortalRepository
    {
        private readonly IRepository _dbRepository;
        private readonly LogService _logger;

        public DoctorPortalRepository(IRepository dbRepository)
        {
            _dbRepository = dbRepository;
            _logger = new LogService();
        }

        public async Task<DoctorLoginResultDto> LoginAsync(CancellationToken cancellationToken, string identifier)
        {
            var parameters = new List<ParametersCollection>
             {
                  new() { ParameterName = "@Identifier", ParameterValue = identifier, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };

            var result = await _dbRepository.ExecuteSpSingleAsync<DoctorLoginResultDto>(
                cancellationToken,
                "sp_DoctorLogin",
                parameters
            );

            return result;
        }

        public async Task<IEnumerable<FetchPatientsByDoctorDto>> FetchPatientsByDoctorAsync(FetchPatientsByDoctorCommand request, CancellationToken cancellationToken) {
            string procedure = "sp_FetchPatientsByDoctor";
            var parameters = new List<ParametersCollection> { 
                new (){ ParameterName = "@DoctorId", ParameterValue = request.DoctorId, ParameterType = DbType.Int64, ParameterDirection = ParameterDirection.Input }
            };
            var result = await _dbRepository.ExecuteSpListAsync<FetchPatientsByDoctorDto>(cancellationToken, procedure, parameters);
            return result;
        }
        public async Task<ViewPatientDto> ViewPatientAsync(ViewPatientQuery request, CancellationToken cancellationToken) {
            string procedure = "sp_ViewPatient";
            var parameters = new List<ParametersCollection> {
                new() { ParameterName = "@PatientId", ParameterValue = request.PatientId, ParameterType = DbType.Int64, ParameterDirection = ParameterDirection.Input},
                new() { ParameterName = "@DoctorId", ParameterValue = request.DoctorId, ParameterType = DbType.Int64, ParameterDirection = ParameterDirection.Input},
            };
            var result = await _dbRepository.ExecuteSpSingleAsync<ViewPatientDto>(cancellationToken, procedure, parameters);
            return result;
        }
        public async Task<IEnumerable<FetchValidTestsDto>> FetchValidTestsAsync(FetchValidTestsQuery request, CancellationToken cancellationToken) {
            string procedure = "sp_FetchValidTests";
            var result = await _dbRepository.ExecuteSpListAsync<FetchValidTestsDto>(cancellationToken, procedure);
            return result;
        }

        public async Task<int> CreateLabRequestAsync(
            CancellationToken cancellationToken,
            LabRequest labRequest,
            DataTable labRequestItemsTvp
        )
        {
            var parameters = new List<ParametersCollection>
            {
                new() { ParameterName = "@PatientId", ParameterValue = labRequest.PatientId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DoctorId", ParameterValue = labRequest.DoctorId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@RequestDate", ParameterValue = labRequest.RequestDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = labRequest.Status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Notes", ParameterValue = labRequest.Notes, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CreatedAt", ParameterValue = labRequest.CreatedAt, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UpdatedAt", ParameterValue = labRequest.UpdatedAt, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = labRequest.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
    
                // TVP parameter
                new()
                {
                    ParameterName = "@Items",
                    ParameterValue = labRequestItemsTvp,
                    ParameterType = DbType.Object,
                    ParameterDirection = ParameterDirection.Input
                },
                new() { ParameterName = "@NewLabRequestId", ParameterType = DbType.Int64, ParameterDirection = ParameterDirection.Output },
            };

            var result = await _dbRepository.ExecuteSpReturnValueAsync(
                cancellationToken,
                "sp_CreateLabRequest",
                parameters
            );

            return (int)result;
        }
        public async Task<IEnumerable<FetchLabRequestsWithItemsByPatientDto>> FetchLabRequestsWithItemsByPatientAsync(FetchLabRequestsWithItemsByPatientCommand request, CancellationToken cancellationToken) {
            string Procedure = "sp_FetchLabRequestsWithItemsByPatient";
            var Parameters = new List<ParametersCollection>
            {
                new (){ParameterName = "@DoctorId", ParameterValue = request.DoctorId, ParameterType=DbType.Int64, ParameterDirection = ParameterDirection.Input},
                new (){ParameterName = "@patientId", ParameterValue = request.PatientId, ParameterType=DbType.Int64, ParameterDirection = ParameterDirection.Input}
            };
            var result = await _dbRepository.ExecuteSpListAsync<FetchLabRequestsWithItemsByPatientDto>(cancellationToken, Procedure, Parameters);
            return result;
        }
    }
}
