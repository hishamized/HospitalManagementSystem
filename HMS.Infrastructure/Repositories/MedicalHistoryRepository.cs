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

    }
}
