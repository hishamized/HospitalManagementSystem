using AutoMapper;
using HMS.Application.Commands.PatientPortal;
using HMS.Application.DTO.PatientPortal;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.PatientPortal
{
    public class SearchPatientByIdentifierHandler : IRequestHandler<SearchPatientByIdentifierQuery, GetPatientByIdentifierDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IMapper _mapper;
        public SearchPatientByIdentifierHandler(IUnitOfWork unitOfWork/*, IMapper mapper*/) { 
            _unitOfWork = unitOfWork;
            //_mapper = mapper;
        }

        public async Task<GetPatientByIdentifierDto> Handle(SearchPatientByIdentifierQuery request, CancellationToken token)
        {
           
            var result = await _unitOfWork.PatientPortalRepository.GetPatientByIdentifierAsync(token, request.Identifier);
            return result;
        }

    }

}
