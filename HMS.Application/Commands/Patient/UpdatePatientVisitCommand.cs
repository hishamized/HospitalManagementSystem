using HMS.Application.Dto;
using HMS.Application.DTOs;
using MediatR;

namespace HMS.Application.Commands.PatientVisits
{
    public class UpdatePatientVisitCommand : IRequest<bool>
    {
        public PatientVisitUpdateDto PatientVisit { get; set; }

        public UpdatePatientVisitCommand(PatientVisitUpdateDto patientVisit)
        {
            PatientVisit = patientVisit;
        }
    }
}
