using AutoMapper;
using HMS.Application.Interfaces;
using HMS.Application.Queries.DoctorDocument;
using HMS.Application.ViewModel.DoctorDocument;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorDocument
{
    public class GetAllDoctorDocumentsWithDoctorsHandler : IRequestHandler<GetAllDoctorDocumentsWithDoctorsQuery, IEnumerable<DoctorDocumentWithDoctorViewModel>>
    {
        private readonly IDoctorDocumentRepository _doctorDocumentRepository;
        private readonly IMapper _mapper;

        public GetAllDoctorDocumentsWithDoctorsHandler(IDoctorDocumentRepository doctorDocumentRepository, IMapper mapper)
        {
            _doctorDocumentRepository = doctorDocumentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DoctorDocumentWithDoctorViewModel>> Handle(GetAllDoctorDocumentsWithDoctorsQuery request, CancellationToken cancellationToken)
        {
            var documents = await _doctorDocumentRepository.GetAllDoctorDocumentsWithDoctorsAsync();

            // Optional AutoMapper use — if repo returns entities instead of VMs
            var mappedResult = _mapper.Map<IEnumerable<DoctorDocumentWithDoctorViewModel>>(documents);

            return mappedResult;
        }
    }
}
