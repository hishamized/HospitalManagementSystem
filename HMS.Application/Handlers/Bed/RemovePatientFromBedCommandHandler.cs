using HMS.Application.Commands.Bed;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bed
{
    public class RemovePatientFromBedCommandHandler : IRequestHandler<RemovePatientFromBedCommand, bool>
    {
        private readonly IBedRepository _repository;

        public RemovePatientFromBedCommandHandler(IBedRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RemovePatientFromBedCommand request, CancellationToken cancellationToken)
        {
            var result = await _repository.RemovePatientFromBedAsync(request.PatientBedWardId);
            return result > 0;
        }
    }
}
