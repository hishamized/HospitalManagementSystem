using HMS.Application.Commands.LabTest;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.LabTest
{
    public class UpdateLabRequestItemStatusCommandHandler : IRequestHandler<UpdateLabRequestItemStatusCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateLabRequestItemStatusCommandHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<long> Handle(UpdateLabRequestItemStatusCommand request, CancellationToken cancellationToken) {
            var result = await _unitOfWork.DoctorPortalRepository.UpdateLabRequestItemStatusAsync(request,cancellationToken);
            return result;
        }
    }
}
