using HMS.Application.DTO.Allergy;
using MediatR;
using System.Collections.Generic;

namespace HMS.Application.Queries.Allergy
{
    public class GetPatientAllergiesQuery : IRequest<IEnumerable<ViewAllergyDto>>
    {
        public int PatientId { get; set; }

        public GetPatientAllergiesQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
