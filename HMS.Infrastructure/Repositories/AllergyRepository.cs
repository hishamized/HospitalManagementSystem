using Dapper;
using HMS.Application.DTO.Allergy;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data; // DapperContext
using System.Data;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class AllergyRepository : IAllergyRepository
    {
        private readonly DapperContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public AllergyRepository(DapperContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddAsync(AddAllergyDto allergy)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(allergy);

            var newId = await conn.QueryFirstAsync<int>(
                "sp_AddAllergy",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newId;
        }

        public async Task<IEnumerable<Allergy>> GetPatientAllergiesAsync(int patientId)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PatientId", patientId, DbType.Int32);

            var allergies = await conn.QueryAsync<Allergy>(
                "sp_GetPatientAllergies",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return allergies;
        }
        public async Task<bool> UpdateAllergyAsync(EditAllergyDto allergy)
        {
            using var conn = _context.CreateConnection();
            var parameters = new DynamicParameters(allergy);

   
            var rows = await conn.ExecuteAsync(
                "sp_UpdateAllergy",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        public async Task<int> DeleteAllergyAsync(int id)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            var rowsAffected = await conn.QueryFirstAsync<dynamic>(
                 "sp_DeleteAllergy",
                 parameters,
                 commandType: CommandType.StoredProcedure
             );

            return (int)rowsAffected.RowsAffected;

        }
    }
}
