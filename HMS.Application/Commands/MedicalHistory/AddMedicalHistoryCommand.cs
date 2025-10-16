using HMS.Application.DTO.MedicalHistory;
using MediatR;

namespace HMS.Application.Commands.MedicalHistory
{
    public class AddMedicalHistoryCommand : IRequest<int>
    {
        public CreateMedicalHistoryDto MedicalHistoryDto { get; }

        public AddMedicalHistoryCommand(CreateMedicalHistoryDto dto)
        {
            MedicalHistoryDto = dto;
        }
    }
}
