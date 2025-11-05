using AutoMapper;
using Dapper;
using HMS.Application.DTO.Bed;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class BedRepository : IBedRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public BedRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> AddBedAsync(AddBedDto dto)
        {
            using var conn = _context.CreateConnection();
            var parameters = new DynamicParameters(dto);
            var result = await conn.ExecuteScalarAsync<int>(
                "sp_AddBed",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return result;
        }

        public async Task<int> CheckBedUniquenessAsync(string bedCode, string bedNumber)
        {
            using var conn = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("BedCode", bedCode);
            parameters.Add("BedNumber", bedNumber);

            return await conn.ExecuteScalarAsync<int>(
                "sp_CheckBedUniqueness",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<(IEnumerable<BedListDto> Beds, int TotalCount)> GetPagedBedsAsync(int pageNumber, int pageSize)
        {
            using var conn = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("PageNumber", pageNumber);
            parameters.Add("PageSize", pageSize);

            using var multi = await conn.QueryMultipleAsync(
                "sp_GetBedsPaged",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var beds = multi.Read<BedListDto>().ToList();
            var totalCount = multi.Read<int>().Single();

            return (beds, totalCount);
        }
        public async Task<int> UpdateBedAsync(EditBedDto dto)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(dto);

            return await conn.ExecuteScalarAsync<int>(
                "sp_UpdateBed",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<int> DeleteBedAsync(int id)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);

            var result = await conn.ExecuteScalarAsync<int>(
                "sp_DeleteBed",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        public async Task<IEnumerable<BedDropdownDto>> GetBedsByWardIdAsync(int wardId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@WardId", wardId, DbType.Int32);

            var beds = await connection.QueryAsync<BedDropdownDto>(
                "sp_GetBedsByWardId",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return beds;
        }
        public async Task<bool> AllotBedAsync(AllotBedDto entity)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters(entity);

            var result = await connection.ExecuteScalarAsync<int>(
                "sp_AllotBedToPatient",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
        public async Task<CheckBedDto> CheckBedStatusAsync(int patientId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("PatientId", patientId);

            var result = await connection.QueryFirstOrDefaultAsync<CheckBedDto>(
                "sp_CheckPatientBedStatus",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result ?? new CheckBedDto
            {
                PatientId = patientId,
                HasBedAllotted = false
            };
        }
        public async Task<int> RemovePatientFromBedAsync(int patientBedWardId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters(new { PatientBedWardId = patientBedWardId });

            var result = await connection.ExecuteScalarAsync<int>(
                "sp_RemovePatientFromBed",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}
