using AutoMapper;
using HMS.Application.Commands.Department;
using HMS.Application.DTO.Department;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Department
{
    public class AddDepartmentHandler : IRequestHandler<AddDepartmentCommand, int>
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;

        public AddDepartmentHandler(IDepartmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddDepartmentCommand request, CancellationToken cancellationToken)
        {
            // Map DTO if needed (for example UI DTO → repository DTO)
            var dto = _mapper.Map<AddDepartmentsDto>(request.DepartmentDto);

            // Call repository to add department and get new ID
            var newDepartmentId = await _repository.AddDepartmentAsync(dto);

            return newDepartmentId;
        }
    }
}
