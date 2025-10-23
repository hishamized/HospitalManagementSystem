using AutoMapper;
using Dapper;
using HMS.Application.DTO.Department;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        public DepartmentRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> AddDepartmentAsync(AddDepartmentsDto dto)
        {
            // Use DynamicParameters to map DTO properties automatically
            var parameters = new DynamicParameters(dto);

            using var conn = _context.CreateConnection();

            // Calls the stored procedure and returns the new department ID
            var newDepartmentId = await conn.ExecuteScalarAsync<int>(
                "sp_AddDepartment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newDepartmentId;
        }
        public async Task<List<DepartmentDto>> GetAllDepartmentsAsync()
        {
            using var conn = _context.CreateConnection();

            var departments = await conn.QueryAsync<DepartmentDto>(
                "sp_GetAllDepartments",
                commandType: CommandType.StoredProcedure
            );

            return departments.AsList();
        }

        public async Task<int> EditDepartmentAsync(EditDepartmentDto dto)
        {
            using var conn = _context.CreateConnection();

            // Map all DTO properties automatically
            var parameters = new DynamicParameters(dto);

            // Execute stored procedure and return number of affected rows
            var affectedRows = await conn.ExecuteAsync(
                "sp_UpdateDepartment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return affectedRows;
        }
        public async Task<int> DeleteDepartmentAsync(int id)
        {
            using var conn = _context.CreateConnection();

            // Calls the stored procedure and returns number of rows affected
            var rowsAffected = await conn.ExecuteScalarAsync<int>(
                "sp_DeleteDepartment",
                new { Id = id },  // Dapper automatically maps anonymous object to parameters
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected;
        }
    }
 
}
