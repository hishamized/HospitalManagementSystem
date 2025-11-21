using AutoMapper;
using HMS.Application.Commands.DoctorPortal;
using HMS.Application.DTO.DoctorPortal;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorPortal
{

    public class CreateLabRequestCommandHandler : IRequestHandler<CreateLabRequestCommand, LabRequestCreateResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateLabRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LabRequestCreateResultDto> Handle(CreateLabRequestCommand request, CancellationToken cancellationToken)
        {
            // Map the LabRequestCreateDto → Domain Entity (if needed)
            var requestEntity = _mapper.Map<Domain.Entities.LabRequest>(request.Request.LabRequest);

            // Convert List<LabRequestItemCreateDto> → DataTable for TVP
            var itemsTable = new DataTable();
            itemsTable.Columns.Add("LabTestId", typeof(int));
            itemsTable.Columns.Add("Status", typeof(string));
            itemsTable.Columns.Add("CreatedAt", typeof(DateTime));
            itemsTable.Columns.Add("UpdatedAt", typeof(DateTime));
            itemsTable.Columns.Add("IsActive", typeof(bool));

            foreach (var item in request.Request.LabRequestItems)
            {
                itemsTable.Rows.Add(
                    item.LabTestId,
                    item.Status,
                    item.CreatedAt,
                    item.UpdatedAt,
                    item.IsActive
                );
            }

            // Call repository (via UoW)
            var newLabRequestId = await _unitOfWork.DoctorPortalRepository.CreateLabRequestAsync(
                cancellationToken,
                requestEntity,
                itemsTable
            );

            return new LabRequestCreateResultDto
            {
                LabRequestId = newLabRequestId,
                Message = "Lab request created successfully"
            };
        }
    }
}
