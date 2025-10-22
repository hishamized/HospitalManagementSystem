using HMS.Application.DTOs.PatientVisitDtos;
using MediatR;
using System.Collections.Generic;

namespace HMS.Application.Features.PatientVisits.Queries
{
    public class GetAllPatientVisitsQuery : IRequest<IEnumerable<PatientVisitDto>>
    {
        // No parameters needed for now, can add filters later
    }
}
