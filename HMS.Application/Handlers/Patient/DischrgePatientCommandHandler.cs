using AutoMapper;
using HMS.Application.Commands.Patient;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Patient
{
    public class DischargePatientCommandHandler : IRequestHandler<DischargePatientCommand, bool>
    {
        private readonly IPatientVisitRepository _repository;

        public DischargePatientCommandHandler(IPatientVisitRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DischargePatientCommand request, CancellationToken cancellationToken)
        {
            bool RowsAffected = await _repository.DischargePatientAsync(request.VisitId);
            return RowsAffected;
        }
    }
}
