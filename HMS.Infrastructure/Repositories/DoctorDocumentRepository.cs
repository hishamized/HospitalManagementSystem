using AutoMapper;
using Dapper;
using HMS.Application.DTO.DoctorDocument;
using HMS.Application.Interfaces;
using HMS.Application.ViewModel.DoctorDocument;
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
    public class DoctorDocumentRepository : IDoctorDocumentRepository
    {
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorDocumentRepository(DapperContext context, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddDoctorDocumentAsync(DoctorDocumentDto dto)
        {
            using var connection = _context.CreateConnection();

            // ✅ Make sure we do NOT pass navigation property
            var parameters = new DynamicParameters(new
            {
                dto.DoctorId,
                dto.FileName,
                dto.FilePath,
                dto.FileType,
                dto.FileSize,
                dto.UploadedAt,
                dto.UploadedBy,
                dto.IsActive
            });

            return await connection.ExecuteScalarAsync<int>(
                "sp_AddDoctorDocument",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<DoctorDocumentWithDoctorViewModel>> GetAllDoctorDocumentsWithDoctorsAsync()
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<DoctorDocumentWithDoctorViewModel>(
                "sp_GetAllDoctorDocumentsWithDoctors",
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        public async Task<bool> UpdateDoctorDocumentAsync(EditDoctorDocumentDto dto)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters(dto);

            // Execute stored procedure and get affected row count
            var affectedRows = await conn.ExecuteScalarAsync<int>(
                "sp_UpdateDoctorDocument",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return affectedRows > 0;
        }

        public async Task<bool> DeleteDoctorDocumentAsync(int documentId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@DocumentId", documentId);
            parameters.Add("ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            // ⚡ Use ExecuteAsync, but capture the RETURN value manually
            await connection.ExecuteAsync(
                "sp_DeleteDoctorDocument",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int rowsAffected = parameters.Get<int>("ReturnValue");
            Console.WriteLine($"Rows affected (returned): {rowsAffected}");

            return rowsAffected > 0;
        }
        public async Task<IEnumerable<GetDoctorDocumentsDto>> GetDoctorDocumentsByDoctorIdAsync(int doctorId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@DoctorId", doctorId, DbType.Int32);

            // Call stored procedure
            var result = await connection.QueryAsync<GetDoctorDocumentsDto>(
                "sp_GetDoctorDocumentsByDoctorId",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}
