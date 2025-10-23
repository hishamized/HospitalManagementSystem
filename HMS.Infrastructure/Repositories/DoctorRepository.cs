using AutoMapper;
using Dapper;
using HMS.Application.Dto;
using HMS.Application.Dto.Doctor;
using HMS.Application.DTO.Doctor;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public DoctorRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> AddDoctorAsync(AddDoctorDto doctor)
        {
            var parameters = new DynamicParameters(doctor);

            using var conn = _context.CreateConnection();

            // Calls the stored procedure
            var newDoctorId = await conn.ExecuteScalarAsync<int>(
                "sp_AddDoctor",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newDoctorId;
        }

        public async Task<IEnumerable<GetDoctorsDto>> GetAllDoctorsAsync()
        {
            using var connection = _context.CreateConnection();

            var doctors = await connection.QueryAsync<GetDoctorsDto>(
                "sp_GetDoctors",
                commandType: CommandType.StoredProcedure
            );

            // AutoMapper step is optional here, but retained for consistency
            return _mapper.Map<IEnumerable<GetDoctorsDto>>(doctors);
        }
        public async Task<bool> UpdateDoctorAsync(EditDoctorDto dto)
        {
            using var connection = _context.CreateConnection();

            // Dapper will automatically map properties of DTO to stored procedure parameters
            var parameters = new DynamicParameters(dto);

            var rowsAffected = await connection.ExecuteScalarAsync<int>(
                "sp_UpdateDoctor",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }
        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            using var connection = _context.CreateConnection();

            // Using DynamicParameters for automatic mapping
            var parameters = new DynamicParameters();
            parameters.Add("@Id", doctorId, DbType.Int32);

            var rowsAffected = await connection.ExecuteScalarAsync<int>(
                "sp_DeleteDoctor",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }
    }
}
