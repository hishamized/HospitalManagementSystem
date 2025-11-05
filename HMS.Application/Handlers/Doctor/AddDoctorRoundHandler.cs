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
    public class AddDoctorRoundHandler : IRequestHandler<AddDoctorRoundCommand, int>
    {
        private readonly IDoctorRepository _doctorRoundRepository;

        public AddDoctorRoundHandler(IDoctorRepository doctorRoundRepository)
        {
            _doctorRoundRepository = doctorRoundRepository;
        }

        public async Task<int> Handle(AddDoctorRoundCommand request, CancellationToken cancellationToken)
        {
            return await _doctorRoundRepository.AddDoctorRoundAsync(request.DoctorRound);
        }
    }
}
