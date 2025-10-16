using HMS.Application.DTO.Patient;
using MediatR;
using System.Collections.Generic;

namespace HMS.Application.Queries
{
    public class GetAllPatientsQuery : IRequest<IEnumerable<PatientDto>>
    {
    }
}
