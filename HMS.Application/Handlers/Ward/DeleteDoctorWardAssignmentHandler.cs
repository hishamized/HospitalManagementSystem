using AutoMapper;
using HMS.Application.Commands.Ward;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Ward
{
    public class DeleteDoctorWardAssignmentHandler : IRequestHandler<DeleteDoctorWardAssignmentCommand, (bool Success, string Message)>
    {
        private readonly IWardRepository _wardRepository;
        private readonly IMapper _mapper;

        public DeleteDoctorWardAssignmentHandler(IWardRepository wardRepository, IMapper mapper)
        {
            _wardRepository = wardRepository;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message)> Handle(DeleteDoctorWardAssignmentCommand request, CancellationToken cancellationToken)
        {
            var result = await _wardRepository.DeleteDoctorWardAssignmentAsync(request.Id);

            if (result > 0)
                return (true, "Doctor assignment deleted successfully.");
            else
                return (false, "Failed to delete doctor assignment or record not found.");
        }
    }
}
