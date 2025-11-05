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
    public class DeleteDoctorRoundCommandHandler : IRequestHandler<DeleteDoctorRoundCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepository;

        public DeleteDoctorRoundCommandHandler(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<bool> Handle(DeleteDoctorRoundCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                return false;

            var result = await _doctorRepository.DeleteDoctorRoundAsync(request.Id);

            return result;
        }
    }
}
