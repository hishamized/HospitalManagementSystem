using MediatR;
using HMS.Application.Commands.Payment;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using AutoMapper;
using Microsoft.Data.SqlClient;

namespace HMS.Application.Handlers.Payment
{
    public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddPaymentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        Task<int> IRequestHandler<AddPaymentCommand, int>.Handle(AddPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = _unitOfWork.PaymentRepository.ProcessPaymentAsync(cancellationToken, request);

                return result;
            }
            catch (SqlException ex)
            {
                // Return a failed Task<int> with a suitable error code, e.g., -1
                return Task.FromResult(-1);
            }
        }
    }
}
