using AutoMapper;
using HMS.Application.Commands.Doctor;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Doctor
{
    public class EditDoctorHandler : IRequestHandler<EditDoctorCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public EditDoctorHandler(IDoctorRepository doctorRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(EditDoctorCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Doctor;

            // Call repository method to update doctor
            var result = await _doctorRepository.UpdateDoctorAsync(dto);

            return result;
        }
    }
}
