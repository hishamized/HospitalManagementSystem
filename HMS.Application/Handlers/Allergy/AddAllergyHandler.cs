using HMS.Application.Commands.Allergy;
using HMS.Application.DTO.Allergy;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Allergy
{
    public class AddAllergyHandler : IRequestHandler<AddAllergyCommand, int>
    {
        private readonly IAllergyRepository _repository;

        public AddAllergyHandler(IAllergyRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(AddAllergyCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddAsync(request.Allergy);
        }
    }
}
