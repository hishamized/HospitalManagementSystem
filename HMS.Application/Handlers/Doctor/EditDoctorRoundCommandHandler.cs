using HMS.Application.Commands.Doctor;
using HMS.Application.DTO.Doctor;
using HMS.Application.Interfaces;
using MediatR;

namespace HMS.Application.Handlers.Doctor
{
    public class EditDoctorRoundCommandHandler : IRequestHandler<EditDoctorRoundCommand, UpdateDoctorRoundDto>
    {
        private readonly IDoctorRepository _doctorRepository;

        public EditDoctorRoundCommandHandler(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<UpdateDoctorRoundDto> Handle(EditDoctorRoundCommand request, CancellationToken cancellationToken)
        {
            if (request.DoctorRound == null)
            {
                return null;
            }

            // Pass DTO directly to repository
            var updatedRound = await _doctorRepository.UpdateDoctorRoundAsync(request.DoctorRound);

            return updatedRound; // returns the updated DTO from the SP
        }
    }
}
