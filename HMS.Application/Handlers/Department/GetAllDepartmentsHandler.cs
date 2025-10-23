using AutoMapper;
using HMS.Application.DTO.Department;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Department;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Department
{

    public class GetAllDepartmentsHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;

        public GetAllDepartmentsHandler(IDepartmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            // Call repository to fetch departments as DTOs
            var departmentDtos = await _repository.GetAllDepartmentsAsync();

            // AutoMapper can be used if needed, here it's already DTO from repository
            return _mapper.Map<List<DepartmentDto>>(departmentDtos);
        }
    }
}
