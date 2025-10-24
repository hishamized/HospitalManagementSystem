using AutoMapper;
using Dapper;
using HMS.Application.DTO.Appointment;
using HMS.Application.Interfaces;
using HMS.Application.ViewModel.Appointment;
using HMS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public AppointmentRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> AddAsync(AddAppointmentDTO appointmentDto)
        {
            var conn = _context.CreateConnection();
            // Use AutoMapper to create DynamicParameters from DTO
            var parameters = _mapper.Map<DynamicParameters>(appointmentDto);

            // Execute stored procedure
            var result = await conn.QuerySingleAsync<int>(
                "sp_AddAppointment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync()
        {
            using var connection = _context.CreateConnection();

            var appointments = await connection.QueryAsync<AppointmentViewModel>(
                "sp_GetAllAppointments",
                commandType: CommandType.StoredProcedure
            );

            return appointments;
        }


        public async Task<bool> RescheduleAppointmentAsync(RescheduleAppointmentDto dto)
        {
            using var conn = _context.CreateConnection();

            var parameters = _mapper.Map<DynamicParameters>(dto);

            int rowsAffected = await conn.ExecuteScalarAsync<int>(
                "sp_RescheduleAppointment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", appointmentId, DbType.Int32);

            // Execute the stored procedure
            int rowsAffected = await conn.ExecuteScalarAsync<int>(
                "sp_DeleteAppointment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

    }
}
