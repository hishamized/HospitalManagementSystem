using HMS.Application.Commands.Payment;
using HMS.Application.Interfaces;
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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IRepository _dbRepository;
        private readonly LogService _logger;

        public PaymentRepository(IRepository dbRepository)
        {
            _dbRepository = dbRepository;
            _logger = new LogService();
        }

        public async Task<int> ProcessPaymentAsync(CancellationToken cancellationToken, AddPaymentCommand request)
        {
            string storedProcedure = "sp_ProcessPayment";
            var parameters = new List<ParametersCollection>{
                new() { ParameterName = "@PatientId", ParameterValue = request.Dto.PatientId, ParameterType = DbType.Int64, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@VisitId", ParameterValue = request.Dto.VisitId, ParameterType = DbType.Int64, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@BillId", ParameterValue = request.Dto.BillId, ParameterType = DbType.Int64, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PaymentDate", ParameterValue = request.Dto.PaymentDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Amount", ParameterValue = request.Dto.Amount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PaymentStatus", ParameterValue = request.Dto.PaymentStatus, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PaymentMode", ParameterValue = request.Dto.PaymentMode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PaymentId", ParameterValue = 0, ParameterType = DbType.Int64, ParameterDirection = ParameterDirection.Output },
            };

            var result = await _dbRepository.ExecuteSpReturnValueAsync(cancellationToken, storedProcedure, parameters);

            return (int)result;
        }
    }
}
