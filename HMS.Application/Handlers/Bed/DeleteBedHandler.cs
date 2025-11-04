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
    public class DeleteBedHandler : IRequestHandler<DeleteBedCommand, int>
    {
        private readonly IBedRepository _repository;

        public DeleteBedHandler(IBedRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(DeleteBedCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid Bed ID provided for deletion.");

            var rowsAffected = await _repository.DeleteBedAsync(request.Id);

            return rowsAffected;
        }
    }
}
