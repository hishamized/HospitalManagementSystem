using HMS.Application.DTO.Insurance;
using MediatR;
using System.Collections.Generic;

namespace HMS.Application.Queries.Insurance
{
    public class GetInsurancesByPatientQuery : IRequest<IEnumerable<ViewInsuranceDto>>
    {
        public int PatientId { get; }

        public GetInsurancesByPatientQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
