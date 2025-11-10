using HMS.Application.DTO.Bill;
using HMS.Application.Commands.Bill;
using MediatR;
using HMS.Application.Interfaces;

namespace HMS.Application.Handlers.Bill
{
    public class GetFinalBillCommandHandler : IRequestHandler<GetFinalBillCommand, GetFinalBillDto>
    {
        public IBillRepository _repository;

        public GetFinalBillCommandHandler(IBillRepository repository) {
            _repository = repository;
        }

        public async Task<GetFinalBillDto> Handle(GetFinalBillCommand request, CancellationToken cancellation) {
            var result = await _repository.GetFinalBillAsync(request.PatientId, request.VisitId);
            return result;
        }
    }
}
