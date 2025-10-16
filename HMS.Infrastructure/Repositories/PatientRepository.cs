using Dapper;
using HMS.Application.DTO.Patient;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DapperContext _context;

        public PatientRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<Patient>("sp_GetAllPatients", commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Patient>(
                "sp_GetPatientById",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure);
        }


        public async Task<int> AddAsync(CreatePatientDto patient)
        {
            using var conn = _context.CreateConnection();

            // Generate PatientCode from SP
            patient.PatientCode = await conn.QueryFirstAsync<string>("sp_GeneratePatientCode", commandType: System.Data.CommandType.StoredProcedure);

            var parameters = new DynamicParameters(patient);

            return await conn.ExecuteAsync("sp_CreatePatient", parameters, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<int> UpdateAsync(UpdatePatientDto patient)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(patient);

            return await conn.ExecuteAsync("sp_UpdatePatient", parameters, commandType: System.Data.CommandType.StoredProcedure);
        }


        public async Task<int> DeleteAsync(int id)
        {
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync("sp_DeletePatient", new { Id = id }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<string> GeneratePatientCodeAsync()
        {
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstAsync<string>("sp_GeneratePatientCode", commandType: System.Data.CommandType.StoredProcedure);
        }

      
    }
}
