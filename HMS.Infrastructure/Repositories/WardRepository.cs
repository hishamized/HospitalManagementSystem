using AutoMapper;
using Dapper;
using HMS.Application.DTO.Ward;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{

    public class WardRepository : IWardRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        public WardRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> AddWardAsync(CreateWardDto dto)
        {
            using var connection = _context.CreateConnection();

            // 🔹 Dynamic parameters auto-map from DTO properties
            var parameters = new DynamicParameters(dto);

            // Execute stored procedure and return new WardId
            var newWardId = await connection.ExecuteScalarAsync<int>(
                "sp_AddWard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newWardId;
        }

        public async Task<IEnumerable<WardDto>> GetAllWardsAsync()
        {
            using var connection = _context.CreateConnection();

            // ✅ No manual parameter.Add — dynamic and clean
            var parameters = new DynamicParameters();

            var result = await connection.QueryAsync<WardDto>(
                "sp_GetAllWards",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        public async Task<int> UpdateWardAsync(UpdateWardDto ward)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters(ward);

            var rowsAffected = await connection.ExecuteAsync(
                "sp_UpdateWard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected;
        }

        public async Task<int> DeleteWardAsync(int wardId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", wardId);

            var result = await connection.ExecuteScalarAsync<int>(
                "sp_DeleteWard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result; // ✅ Correctly returns 1 if row deleted, 0 if not found
        }

    }
}
