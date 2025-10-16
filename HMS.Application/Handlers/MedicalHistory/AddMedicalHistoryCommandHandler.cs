using AutoMapper;
using HMS.Application.Commands.MedicalHistory;
using HMS.Application.DTO.MedicalHistory;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HMS.Application.Interfaces;

namespace HMS.Application.Handlers.MedicalHistory
{
    public class AddMedicalHistoryCommandHandler : IRequestHandler<AddMedicalHistoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;
        private readonly IMapper _mapper;

        public AddMedicalHistoryCommandHandler(
            IUnitOfWork unitOfWork,
            IMedicalHistoryRepository medicalHistoryRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _medicalHistoryRepository = medicalHistoryRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddMedicalHistoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<CreateMedicalHistoryDto>(request.MedicalHistoryDto);
            var id = await _medicalHistoryRepository.AddAsync(entity);
            return id;
        }
    }
}
