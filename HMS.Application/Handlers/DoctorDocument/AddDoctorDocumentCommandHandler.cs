using AutoMapper;
using HMS.Application.Commands.DoctorDocument;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorDocument
{
    public class AddDoctorDocumentCommandHandler : IRequestHandler<AddDoctorDocumentCommand, int>
    {
        private readonly IDoctorDocumentRepository _doctorDocumentRepository;
        private readonly IMapper _mapper;

        public AddDoctorDocumentCommandHandler(IDoctorDocumentRepository doctorDocumentRepository, IMapper mapper)
        {
            _doctorDocumentRepository = doctorDocumentRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddDoctorDocumentCommand request, CancellationToken cancellationToken)
        {
            // Map DTO to entity (if needed)
            var doctorDocumentDto = request.DoctorDocument;

            // Call repository method (which will internally execute the stored procedure)
            var newDocumentId = await _doctorDocumentRepository.AddDoctorDocumentAsync(doctorDocumentDto);

            return newDocumentId;
        }
    }
}
