using HMS.Application.Commands.DoctorPortal;
using HMS.Application.DTO.DoctorPortal;
using HMS.Application.DTO.PatientPortal;
using HMS.Application.Interfaces;
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
    }
}
