using HMS.Application.DTO.PatientPortal;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Exceptions;
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
    public class PatientPortalRepository : IPatientPortalRepository
    {
        private readonly IRepository _dbRepository;
        private readonly LogService _logger;

        public PatientPortalRepository(IRepository dbRepository) {
            _dbRepository = dbRepository;
            _logger = new LogService();
        }

        public async Task<GetPatientByIdentifierDto?> GetPatientByIdentifierAsync(
     CancellationToken token,
     string identifier)
        {
            try
            {
                var parameters = new List<ParametersCollection>
        {
            new()
            {
                ParameterName = "@identifier",
                ParameterValue = identifier,
                ParameterType = DbType.String,
                ParameterDirection = ParameterDirection.Input
            }
        };

                var result = await _dbRepository.ExecuteSpSingleAsync<GetPatientByIdentifierDto>(
                    token,
                    "sp_SearchPatientPortal",
                    parameters
                );

                return result;
            }
            catch (StoredProcedureExecutionException ex)
            {
                // Gracefully handle "No Records Found" without breaking the flow
                if (ex.Message.Contains("No Records Found", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                // For other DB-related errors, rethrow or handle separately
                throw;
            }
            catch (Exception ex)
            {
                // Log or rethrow unexpected exceptions if needed
                throw new Exception("An unexpected error occurred while fetching patient data.", ex);
            }
        }

    }
}
