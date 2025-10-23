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
    public class DeleteDoctorHandler : IRequestHandler<DeleteDoctorCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepository;

        public DeleteDoctorHandler(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<bool> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            // Call repository to delete doctor by ID
            var result = await _doctorRepository.DeleteDoctorAsync(request.DoctorId);
            return result;
        }
    }
}
