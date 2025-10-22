using AutoMapper;
using Dapper;
using HMS.Application.Dto;
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
    }
}
