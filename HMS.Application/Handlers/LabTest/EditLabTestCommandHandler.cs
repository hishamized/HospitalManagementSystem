using HMS.Application.Commands.LabTest;
using HMS.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.LabTest
{
    public class EditLabTestCommandHandler : IRequestHandler<EditLabTestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EditLabTestCommandHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(EditLabTestCommand request, CancellationToken cancellationToken) {
            var result = await _unitOfWork.LabRepository.EditLabTestAsync(request, cancellationToken);
            return result;
        }
    }
}
