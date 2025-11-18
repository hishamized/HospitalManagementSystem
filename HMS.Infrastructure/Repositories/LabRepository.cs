using HMS.Application.Commands.LabTest;
using HMS.Application.DTO.LabTest;
using HMS.Application.Interfaces;
using HMS.Application.Queries.LabTest;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using HMS.Domain.Logger.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class LabRepository : ILabRepository
    {
        private readonly IRepository _dbRepository;
        private readonly LogService _logger;
        public LabRepository(IRepository dbRepository) { 
            _dbRepository = dbRepository;
            _logger = new LogService();
        }
        public async Task<long> AddLabTestAsync(AddLabTestDto request, CancellationToken cancellationtoken) {
            string procedure = "sp_AddNewLabTest";
            var parameters = new List<ParametersCollection> { 
                new (){ ParameterName = "@TestName", ParameterValue = request.TestName, ParameterType = DbType.String , ParameterDirection = ParameterDirection.Input},
                new (){ ParameterName = "@Description", ParameterValue = request.Description, ParameterType = DbType.String , ParameterDirection = ParameterDirection.Input},
                new (){ ParameterName = "@SampleType", ParameterValue = request.SampleType, ParameterType = DbType.String , ParameterDirection = ParameterDirection.Input},
                new (){ ParameterName = "@NormalRange", ParameterValue = request.NormalRange, ParameterType = DbType.String , ParameterDirection = ParameterDirection.Input},
                new (){ ParameterName = "@Price", ParameterValue = request.Price, ParameterType = DbType.Decimal , ParameterDirection = ParameterDirection.Input},
                new (){ ParameterName = "@CreatedAt", ParameterValue = request.CreatedAt, ParameterType = DbType.DateTime2 , ParameterDirection = ParameterDirection.Input},
                new (){ ParameterName = "@UpdatedAt", ParameterValue = request.UpdatedAt, ParameterType = DbType.DateTime2 , ParameterDirection = ParameterDirection.Input},
                new (){ ParameterName = "@IsActive", ParameterValue = request.IsActive, ParameterType = DbType.Boolean , ParameterDirection = ParameterDirection.Input},
            };

            var result = await _dbRepository.ExecuteSpReturnValueAsync(cancellationtoken, procedure, parameters);
            return result;
        }
        public async Task<IEnumerable<FetchLabTestsDto>> FetchLabTestsAsync(FetchLabTestsQuery request, CancellationToken cancellationToken) {
            var procedure = "sp_FetchLabTests";
            IEnumerable<FetchLabTestsDto> result = await _dbRepository.ExecuteSpListAsync<FetchLabTestsDto>(cancellationToken, procedure);
            return result;
        }
    }
}
