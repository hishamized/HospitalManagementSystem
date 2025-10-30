using AutoMapper;
using HMS.Application.Commands.DoctorDocument;
using HMS.Application.DTO.DoctorDocument;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorDocument
{
    public class GetDoctorDocumentsByDoctorIdHandler : IRequestHandler<GetDoctorDocumentsByDoctorIdCommand, IEnumerable<GetDoctorDocumentsDto>>
    {
        private readonly IDoctorDocumentRepository _doctorDocumentRepository;
        private readonly IMapper _mapper;

        public GetDoctorDocumentsByDoctorIdHandler(IDoctorDocumentRepository doctorDocumentRepository, IMapper mapper)
        {
            _doctorDocumentRepository = doctorDocumentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetDoctorDocumentsDto>> Handle(GetDoctorDocumentsByDoctorIdCommand request, CancellationToken cancellationToken)
        {
            // Fetch the data from the repository (Dapper)
            var documents = await _doctorDocumentRepository.GetDoctorDocumentsByDoctorIdAsync(request.DoctorId);

            // Mapping may not be necessary if repository already returns DTOs,
            // but we’ll keep this for consistency
            return _mapper.Map<IEnumerable<GetDoctorDocumentsDto>>(documents);
        }
    }
}
