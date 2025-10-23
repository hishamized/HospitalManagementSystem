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
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentCommand, int>
    {
        private readonly IDepartmentRepository _repository;

        public DeleteDepartmentHandler(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            // Call repository method to delete department
            var rowsAffected = await _repository.DeleteDepartmentAsync(request.Id);

            return rowsAffected;
        }
    }
}
