using HMS.Application.DTO.MedicalHistory;
using MediatR;

namespace HMS.Application.Commands.MedicalHistory
{
    public class UpdateMedicalHistoryCommand : IRequest<bool>
    {
        public EditMedicalHistoryDto MedicalHistory { get; set; }

        public UpdateMedicalHistoryCommand(EditMedicalHistoryDto medicalHistory)
        {
            MedicalHistory = medicalHistory;
        }
    }
}
