using AutoMapper;
using HMS.Application.DTO.LabTest;
using HMS.Application.Queries.LabTest;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.LabTest
{
    public class FetchLabTestsQueryHandler : IRequestHandler<FetchLabTestsQuery, IEnumerable<FetchLabTestsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FetchLabTestsQueryHandler(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitOfWork = unitofwork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<FetchLabTestsDto>> Handle(FetchLabTestsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<FetchLabTestsDto> result = await _unitOfWork.LabRepository.FetchLabTestsAsync(request, cancellationToken);
            return result;
        }
    }

}