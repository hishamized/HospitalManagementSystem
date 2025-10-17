using AutoMapper;
using HMS.Application.Commands.MedicalHistory;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.MedicalHistory
{
    public class UpdateMedicalHistoryHandler : IRequestHandler<UpdateMedicalHistoryCommand, bool>
    {
        private readonly IMedicalHistoryRepository _repository;
        private readonly IMapper _mapper;

        public UpdateMedicalHistoryHandler(IMedicalHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateMedicalHistoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<EditMedicalHistoryDto>(request.MedicalHistory);
            var updatedRows = await _repository.UpdateMedicalHistoryAsync(entity);
            return updatedRows > 0;
        }
    }
}
