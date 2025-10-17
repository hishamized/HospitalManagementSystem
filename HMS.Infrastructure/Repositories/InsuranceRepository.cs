using Dapper;
using HMS.Application.DTO.Insurance;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;

        public InsuranceRepository(IUnitOfWork unitOfWork, DapperContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<int> AddAsync(AddInsuranceDto dto)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(dto);

            var newId = await conn.QueryFirstAsync<int>(
                "sp_AddInsurance",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newId;
        }

        public async Task<IEnumerable<Insurance>> GetByPatientIdAsync(int patientId)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PatientId", patientId, DbType.Int32);

            var insurances = await conn.QueryAsync<Insurance>(
                "sp_GetInsurancesByPatient", // SP to fetch all insurances for patient
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return insurances;
        }

        //public async Task<int> UpdateInsuranceAsync(EditInsuranceDto dto)
        //{
        //    using var conn = _context.CreateConnection();

        //    var parameters = new DynamicParameters(dto);

        //    return await conn.ExecuteAsync(
        //        "sp_UpdateInsurance",
        //        parameters,
        //        commandType: CommandType.StoredProcedure
        //    );
        //}

        //public async Task<int> DeleteInsuranceAsync(int id)
        //{
        //    using var conn = _context.CreateConnection();

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@Id", id, DbType.Int32);

        //    var rowsAffected = await conn.QueryFirstAsync<dynamic>(
        //         "sp_DeleteInsurance",
        //         parameters,
        //         commandType: CommandType.StoredProcedure
        //     );

        //    return (int)rowsAffected.RowsAffected;
        //}
    }
}
