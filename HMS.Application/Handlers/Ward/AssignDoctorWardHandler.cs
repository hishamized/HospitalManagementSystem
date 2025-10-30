using AutoMapper;
using HMS.Application.Commands.Ward;
using HMS.Application.DTO.Ward;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Ward
{
    public class AssignDoctorWardHandler : IRequestHandler<AssignDoctorWardCommand, (bool IsSuccess, string Message)>
    {
        private readonly IMapper _mapper;
        private readonly IWardRepository _wardRepository;

        public AssignDoctorWardHandler(IMapper mapper, IWardRepository wardRepository)
        {
            _mapper = mapper;
            _wardRepository = wardRepository;
        }

        public async Task<(bool IsSuccess, string Message)> Handle(AssignDoctorWardCommand request, CancellationToken cancellationToken)
        {
            var dto = request.AssignDoctorWardDto;

            // Map if necessary (not always required if repository directly uses DTO)
            var doctorWardDto = _mapper.Map<AssignDoctorWardDto>(dto);

            // Call repository method that executes stored procedure
            var result = await _wardRepository.AssignDoctorToWardAsync(doctorWardDto);

            return result; // tuple: (IsSuccess, Message)
        }
    }
}
