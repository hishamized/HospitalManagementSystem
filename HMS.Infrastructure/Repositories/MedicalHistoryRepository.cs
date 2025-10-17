using Dapper;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class MedicalHistoryRepository : IMedicalHistoryRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;

        public MedicalHistoryRepository(IUnitOfWork unitOfWork, DapperContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }


        public async Task<IEnumerable<MedicalHistory>> GetPatientMedicalHistoryAsync(int patientId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PatientId", patientId, DbType.Int32);

            var histories = await connection.QueryAsync<MedicalHistory>(
                "sp_GetPatientMedicalHistory",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return histories;
        }

        public async Task<int> AddAsync(CreateMedicalHistoryDto dto)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(dto);

            var newId = await conn.QueryFirstAsync<int>(
                "sp_AddMedicalHistory",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newId;
        }
        public async Task<int> UpdateMedicalHistoryAsync(EditMedicalHistoryDto medicalHistory)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(medicalHistory);
            return await conn.ExecuteAsync(
                "sp_UpdateMedicalHistory",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<int> DeleteMedicalHistoryAsync(int id)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            var rowsAffected = await conn.QueryFirstAsync<dynamic>(
                 "sp_DeleteMedicalHistory",
                 parameters,
                 commandType: CommandType.StoredProcedure
             );

            return (int)rowsAffected.RowsAffected;

        }
    }
}
