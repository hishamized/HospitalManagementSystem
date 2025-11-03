using AutoMapper;
using Dapper;
using HMS.Application.DTO.Bill;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using System.Data;


namespace HMS.Infrastructure.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        public BillRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }


        public async Task<int> AddBillAsync(AddBillDto billDto)
        {
            try
            {
                using var connection = _context.CreateConnection();

                // Pass DTO directly to DynamicParameters for simplicity
                var parameters = new DynamicParameters(billDto);
           // Execute stored procedure and return generated Bill ID
                var newBillId = await connection.ExecuteScalarAsync<int>(
                    "sp_AddBill",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return newBillId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while inserting bill record.", ex);
            }
        }

        public async Task<IEnumerable<BillListDto>> GetBillsByPatientIdAsync(int patientId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PatientId", patientId, DbType.Int32);

            var result = await connection.QueryAsync<BillListDto>(
                "sp_GetBillsByPatientId",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        public async Task<int> UpdateBillAsync(EditBillDto dto)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(dto);
            parameters.Add("UpdatedAt", DateTime.UtcNow);

            try
            {
                var result = await conn.QuerySingleAsync<int>(
                    "sp_UpdateBill",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result; // should be 1 if update succeeded
            }
            catch (Exception ex)
            {
                throw new Exception($"Database update failed: {ex.Message}", ex);
            }
        }
        public async Task<int> DeleteBillAsync(int id)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            try
            {
                // Execute the stored procedure
                var rowsAffected = await conn.ExecuteAsync(
                    "sp_DeleteBill",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return rowsAffected; // Return number of rows deleted
            }
            catch (Exception ex)
            {
                throw new Exception($"Database delete failed: {ex.Message}", ex);
            }
        }
    }
}
