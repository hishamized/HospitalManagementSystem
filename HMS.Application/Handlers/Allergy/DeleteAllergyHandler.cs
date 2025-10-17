using HMS.Application.Commands.Allergy;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Allergy
{
    public class DeleteAllergyHandler : IRequestHandler<DeleteAllergyCommand, bool>
    {
        private readonly IAllergyRepository _repository;

        public DeleteAllergyHandler(IAllergyRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteAllergyCommand request, CancellationToken cancellationToken)
        {
            var deletedRows = await _repository.DeleteAllergyAsync(request.Id);
            return deletedRows > 0;
        }
    }
}
