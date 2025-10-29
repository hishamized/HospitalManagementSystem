using HMS.Application.Commands.Ward;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Ward
{
    public class DeleteWardCommandHandler : IRequestHandler<DeleteWardCommand, int>
    {
        private readonly IWardRepository _wardRepository;

        public DeleteWardCommandHandler(IWardRepository wardRepository)
        {
            _wardRepository = wardRepository ?? throw new ArgumentNullException(nameof(wardRepository));
        }

        public async Task<int> Handle(DeleteWardCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid Ward ID for deletion.");

            var rowsAffected = await _wardRepository.DeleteWardAsync(request.Id);

            return rowsAffected;
        }
    }
}
