using AutoMapper;
using Dapper;
using HMS.Application.Dto.Role;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public RoleRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetRoleDto>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();

            // Execute stored procedure
            var roles = await connection.QueryAsync<Role>(
                "sp_GetRoles",
                commandType: System.Data.CommandType.StoredProcedure
            );

            // Map Role → GetRoleDto using AutoMapper
            var roleDtos = _mapper.Map<IEnumerable<GetRoleDto>>(roles);

            return roleDtos;
        }


        public async Task<int> AddAsync(AddRoleDto role)
        {
            using var connection = _context.CreateConnection();

            // Automatically map properties using DynamicParameters
            var parameters = new DynamicParameters(role);

            // Execute stored procedure (no transaction required)
            var rowsAffected = await connection.QuerySingleAsync<int>(
                "sp_AddCustomRole",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return rowsAffected;
        }
        public async Task<(bool Success, string Message, int RowsAffected)> DeleteAsync(int roleId)
        {
            using var connection = _context.CreateConnection();

            // Create dynamic parameters
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", roleId);

            // Execute and read the result (Success, Message, RowsAffected)
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "dbo.sp_DeleteRoleCustom",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            // If result is null (shouldn't happen normally)
            if (result == null)
                return (false, "Unexpected error: no response from database.", 0);

            // Map results safely
            bool success = result.Success;
            string message = result.Message;
            int rowsAffected = result.RowsAffected;

            return (success, message, rowsAffected);
        }

        public async Task<int> EditRoleAsync(EditRoleDto role)
        {
            using var connection = _context.CreateConnection();

            // Automatically map DTO properties to stored procedure parameters
            var parameters = new DynamicParameters(role);

            // Execute stored procedure
            var rowsAffected = await connection.QuerySingleAsync<int>(
                "sp_EditRoleCustom",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return rowsAffected;
        }
    }

}