using AutoMapper;
using HMS.Application.Commands.Department;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Department
{
    public class EditDepartmentCommandHandler : IRequestHandler<EditDepartmentCommand, (bool Success, string Message)>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public EditDepartmentCommandHandler(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message)> Handle(EditDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _departmentRepository.EditDepartmentAsync(request.Dto);
                if (result > 0)
                    return (true, "Department updated successfully.");

                return (false, "No records were updated. Please check the Department ID.");
            }
            catch (Exception ex)
            {
                // Optionally log exception here
                return (false, $"An error occurred while updating department: {ex.Message}");
            }
        }
    }
}
