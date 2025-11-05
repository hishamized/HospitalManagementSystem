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
    public class GetDoctorsByPatientHandler : IRequestHandler<GetDoctorsByPatientQuery, IEnumerable<DoctorSelectDto>>
    {
        private readonly IDoctorRepository _doctorRepository;

        public GetDoctorsByPatientHandler(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<IEnumerable<DoctorSelectDto>> Handle(GetDoctorsByPatientQuery request, CancellationToken cancellationToken)
        {
            return await _doctorRepository.GetDoctorsByPatientWardAsync(request.PatientId);
        }
    }
}
