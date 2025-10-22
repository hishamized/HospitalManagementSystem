using HMS.Application.DTOs.PatientVisitDtos;
using MediatR;

namespace HMS.Application.Features.PatientVisits.Commands
{
    public class AddPatientVisitCommand : IRequest<int>
    {
        public AddPatientVisitDto PatientVisit { get; set; }

        public AddPatientVisitCommand(AddPatientVisitDto patientVisit)
        {
            PatientVisit = patientVisit;
        }
    }
}
