using AutoMapper;
using Dapper;
using HMS.Application.DTO.Slot;
using HMS.Application.DTOs.Slot;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class SlotRepository : ISlotRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public SlotRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _context = context;
        }

        public async Task<int> AddAsync(AddSlotDto dto)
        {
            using var conn = _context.CreateConnection();

            // Map all DTO properties automatically to stored procedure parameters
            var parameters = new DynamicParameters(dto);

            // Calls the stored procedure and returns number of affected rows or new ID
            var newSlotId = await conn.ExecuteScalarAsync<int>(
                "sp_AddSlot",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newSlotId;
        }

        public async Task<List<SlotDto>> GetAllAsync()
        {
            using var conn = _context.CreateConnection();

            var slots = await conn.QueryAsync<SlotDto>(
                "sp_GetAllSlots",
                commandType: CommandType.StoredProcedure
            );

            return slots.AsList();
        }
        public async Task<int> EditAsync(EditSlotDto dto)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(dto); // maps all DTO properties automatically

            // Execute stored procedure
            var rowsAffected = await conn.ExecuteScalarAsync<int>(
                "sp_UpdateSlot",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _context.CreateConnection()) // Fixed parentheses
            {

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id, DbType.Int32);

                int rowsAffected = await connection.ExecuteScalarAsync<int>(
                    "sp_DeleteSlot",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return rowsAffected > 0;
            }
        }

    }
}
