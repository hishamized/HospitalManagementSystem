using AutoMapper;
using HMS.Application.DTO.Ward;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Ward;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Ward
{
    public class GetAllWardsQueryHandler : IRequestHandler<GetAllWardsQuery, IEnumerable<WardDto>>
    {
        private readonly IInsuranceRepository _insuranceRepository; // Typo? We'll use IWardRepository below
        private readonly IWardRepository _wardRepository;
        private readonly IMapper _mapper;

        public GetAllWardsQueryHandler(IWardRepository wardRepository, IMapper mapper)
        {
            _wardRepository = wardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WardDto>> Handle(GetAllWardsQuery request, CancellationToken cancellationToken)
        {
            var wards = await _wardRepository.GetAllWardsAsync();

            // AutoMapper will map entity or Dapper dynamic result to DTO
            var wardDtos = _mapper.Map<IEnumerable<WardDto>>(wards);

            return wardDtos;
        }
    }
}
