using AutoMapper;
using Dapper;
using HMS.Application.Dto;
using HMS.Application.DTO;
using HMS.Application.DTO.Insurance;
using HMS.Application.DTOs.PatientVisitDtos;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class PatientVisitRepository : IPatientVisitRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public PatientVisitRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> AddPatientVisitAsync(AddPatientVisitDto dto)
        {
            using var conn = _context.CreateConnection();

            // Pass the entire DTO to DynamicParameters
            var parameters = new DynamicParameters(dto);

            // Execute stored procedure and return the new Visit ID
            var newVisitId = await conn.ExecuteScalarAsync<int>(
                "sp_AddPatientVisit",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return newVisitId;
        }

        public async Task<IEnumerable<PatientVisitDto>> GetAllPatientVisitsAsync()
        {
            using var conn = _context.CreateConnection();

            var visits = await conn.QueryAsync<PatientVisitDto>(
                "sp_GetPatientVisits",
                commandType: System.Data.CommandType.StoredProcedure
            );

            // Map if repository returned entities instead of DTO
            return _mapper.Map<IEnumerable<PatientVisitDto>>(visits);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(new { Id = id });
            var rowsAffected = await conn.ExecuteScalarAsync<int>("sp_DeletePatientVisit", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return rowsAffected > 0;
        }


        public async Task<int> UpdateAsync(PatientVisitUpdateDto dto)
        {
            using var conn = _context.CreateConnection();
            var parameters = new DynamicParameters(dto); // maps properties automatically

            // Execute SP
            var rowsAffected = await conn.QuerySingleAsync<int>(
                "sp_UpdatePatientVisit",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected;
        }
    }
}
