using AutoMapper;
using Dapper;
using HMS.Application.DTO.Feedback;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;
using System.Data;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public FeedbackRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Persists feedback using stored procedure 'sp_AddFeedback' and returns the new feedback Id.
        /// Maps the incoming DTO to the Feedback entity and uses Dapper DynamicParameters.
        /// </summary>
        public async Task<int> AddFeedbackAsync(CreateFeedbackDto dto)
        {
            // map DTO -> Domain entity (keeps naming consistent with DB/SP expectations if needed)
            var entity = _mapper.Map<CreateFeedbackDto>(dto);

            using var conn = _context.CreateConnection();

            // pass entire entity to DynamicParameters (matches pattern used across repositories)
            var parameters = new DynamicParameters(entity);

            // execute stored procedure and return newly created Id
            var newId = await conn.QueryFirstAsync<int>(
                "sp_AddFeedback",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newId;
        }
        public async Task<IEnumerable<FeedbackListDto>> GetAllFeedbacksAsync()
        {
            using var conn = _context.CreateConnection();

            // Execute stored procedure
            var result = await conn.QueryAsync<FeedbackListDto>(
                "sp_GetAllFeedbacks",
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<int> DeleteFeedbackAsync(int id)
        {
            using var conn = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            // Call the stored procedure
            var rowsAffected = await conn.QueryFirstAsync<int>(
                "sp_DeleteFeedback",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected;
        }
    }
}
