using AutoMapper;
using Dapper;
using HMS.Application.Dto.Role;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using HMS.Domain.Logger.Services;
using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        //private readonly DapperContext _context;
        private readonly IMapper _mapper;
        private readonly IRepository _dbRepository;
        public readonly LogService _logger;
        //private UnitOfWork unitOfWork;

        public RoleRepository(IRepository dbRepository, IMapper mapper)
        {
            _dbRepository = dbRepository;
            _mapper = mapper;
            _logger = new LogService();
        }

        public async Task<IEnumerable<GetRoleDto>> GetAllAsync(CancellationToken token)
        {

            try
            {
                var roles = await _dbRepository.ExecuteSpListAsync<Role>(token, "sp_GetRoles", null);

            // Map Role → GetRoleDto using AutoMapper
            var roleDtos = _mapper.Map<IEnumerable<GetRoleDto>>(roles);

                return roleDtos;
            } catch (Exception ex)
            {
                var message = $"Error in {nameof(GetAllAsync)}: {ex.Message}";
                _logger.Error(ex, message);
                return default;
            }
        }


        public async Task<long> AddAsync(CancellationToken cancellationToken, AddRoleDto role)
        {

            var parameters = new List<ParametersCollection> {
                new(){ ParameterName = "@Name", ParameterValue = role.Name, ParameterType = DbType.String , ParameterDirection = ParameterDirection.Input},
                new(){ ParameterName = "@Description", ParameterValue = role.Description, ParameterType = DbType.String , ParameterDirection = ParameterDirection.Input}

            };

            var resultRoleId = await _dbRepository.ExecuteSpReturnValueAsync(cancellationToken, "sp_AddCustomRole", parameters);

            return resultRoleId;
        }
        public async Task<(bool Success, string Message, int RowsAffected)> DeleteAsync(CancellationToken token, int roleId)
        {
            var parameters = new List<ParametersCollection>
                {
                    new()
                    {
                        ParameterName = "@RoleId",
                        ParameterValue = roleId,
                        ParameterType = DbType.Int32,
                        ParameterDirection = ParameterDirection.Input
                    }
                };

            // Execute and get the result set (list of dynamic)
            var result = await _dbRepository.ExecuteSpListAsync<dynamic>(
                CancellationToken.None,
                "sp_DeleteRoleCustom",
                parameters
            );

            // If result is null or empty
            var record = result?.FirstOrDefault();
            if (record == null)
                return (false, "Unexpected error: no response from database.", 0);

            // Safely read dynamic properties
            bool success = (bool)(record.Success ?? false);
            string message = (string)(record.Message ?? "No message returned.");
            int rowsAffected = (int)(record.RowsAffected ?? 0);

            return (success, message, rowsAffected);
        }


        public async Task<int> EditRoleAsync(CancellationToken token, EditRoleDto role)
        {
            try
            {
                var parameters = new List<ParametersCollection> {
            new(){ ParameterName = "@Id", ParameterValue = role.Id, ParameterType = DbType.Int32 , ParameterDirection = ParameterDirection.Input},
            new(){ ParameterName = "@Name", ParameterValue = role.Name, ParameterType = DbType.String , ParameterDirection = ParameterDirection.Input},
            new(){ ParameterName = "@Description", ParameterValue = role.Description, ParameterType = DbType.String , ParameterDirection = ParameterDirection.Input}
                 };
                // Execute stored procedure
                var rowsAffected = await _dbRepository.ExecuteSpReturnValueAsync(
                    token,
                    "sp_EditRoleCustom",
                    parameters
                );

                return (int)rowsAffected;
            }
            catch (Exception ex)
            {
                var message = $"Error in {nameof(EditRoleAsync)}: {ex.Message}";
                _logger.Error(ex, message);
                return -1;
            }
        }
    }


}