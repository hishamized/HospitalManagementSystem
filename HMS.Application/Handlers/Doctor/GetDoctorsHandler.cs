using AutoMapper;
using HMS.Application.Dto.Doctor;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Doctor;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Doctor
{
    public class GetDoctorsHandler : IRequestHandler<GetDoctorsQuery, IEnumerable<GetDoctorsDto>>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public GetDoctorsHandler(IDoctorRepository doctorRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetDoctorsDto>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
        {
            // Fetch from repository (already returns DTOs)
            var doctors = await _doctorRepository.GetAllDoctorsAsync();

            // Mapping (kept for consistency — in case repository returns entity later)
            return _mapper.Map<IEnumerable<GetDoctorsDto>>(doctors);
        }
    }
}
