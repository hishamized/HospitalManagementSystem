using HMS.Application.DTO.Doctor;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Doctor;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Doctor
{
    public class GetDoctorRoundsByPatientHandler : IRequestHandler<GetDoctorRoundsByPatientQuery, IEnumerable<DoctorRoundHistoryDto>>
    {
        private readonly IDoctorRepository _repository;

        public GetDoctorRoundsByPatientHandler(IDoctorRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DoctorRoundHistoryDto>> Handle(GetDoctorRoundsByPatientQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetRoundsByPatientAsync(request.PatientId);
        }
    }
}
