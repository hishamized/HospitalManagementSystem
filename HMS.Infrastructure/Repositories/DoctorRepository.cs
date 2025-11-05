using AutoMapper;
using Dapper;
using HMS.Application.Dto;
using HMS.Application.Dto.Doctor;
using HMS.Application.DTO.Doctor;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using System.Data;
using System.Data.Common;
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
        public async Task<IEnumerable<DoctorSelectDto>> GetDoctorsByPatientWardAsync(int patientId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<DoctorSelectDto>(
                "sp_GetDoctorsByPatientWard",
                new { PatientId = patientId },
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<int> AddDoctorRoundAsync(AddDoctorRoundDto dto)
        {
            using var connection = _context.CreateConnection();

            // Dapper can map the DTO directly to SP parameters
            var newId = await connection.ExecuteScalarAsync<int>(
                "sp_AddDoctorRound",
                param: dto,                  // Auto mapping: dto property names must match SP parameter names
                commandType: CommandType.StoredProcedure
            );

            return newId;
        }
        public async Task<IEnumerable<DoctorRoundHistoryDto>> GetRoundsByPatientAsync(int patientId)
        {
            using var connection = _context.CreateConnection();

            var rounds = await connection.QueryAsync<DoctorRoundHistoryDto>(
                "sp_GetDoctorRoundsByPatient",
                new { PatientId = patientId },
                commandType: CommandType.StoredProcedure
            );

            return rounds;
        }
        public async Task<UpdateDoctorRoundDto?> UpdateDoctorRoundAsync(UpdateDoctorRoundDto doctorRound)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters(doctorRound);

            // Use QuerySingleOrDefaultAsync to get the updated row
            var updatedRound = await connection.QuerySingleOrDefaultAsync<UpdateDoctorRoundDto>(
                "sp_UpdateDoctorRound",   // Stored procedure name
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return updatedRound;
        }

        public async Task<bool> DeleteDoctorRoundAsync(int id)
        {

            if (id <= 0)
                return false;
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            // Execute the stored procedure
            var result = await connection.QuerySingleOrDefaultAsync<int?>(
                "sp_DeleteDoctorRound",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
    }
}
