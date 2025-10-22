using AutoMapper;
using HMS.Application.Dto;
using HMS.Application.DTO.Doctor;
using HMS.Application.Features.Doctors.Commands;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Features.Doctors.Handlers
{
    public class AddDoctorHandler : IRequestHandler<AddDoctorCommand, int>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public AddDoctorHandler(IDoctorRepository doctorRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddDoctorCommand request, CancellationToken cancellationToken)
        {
            // Map DTO to DTO (or entity if repository requires)
            var doctorDto = _mapper.Map<AddDoctorDto>(request.Doctor);

            // Call repository which executes the SP
            var newDoctorId = await _doctorRepository.AddDoctorAsync(doctorDto);

            return newDoctorId;
        }
    }
}
