using AutoMapper;
using HMS.Application.Queries.LabTest;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.LabTest
{
    public class DeleteLabTestQueryHandler : IRequestHandler<DeleteLabTestQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteLabTestQueryHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteLabTestQuery request, CancellationToken cancellationToken) {
            bool result = await _unitOfWork.LabRepository.DeleteLabTestAsync(request, cancellationToken);
            return result;
        }
    }
}
