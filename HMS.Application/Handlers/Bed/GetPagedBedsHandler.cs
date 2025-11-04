using AutoMapper;
using HMS.Application.DTO.Bed;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Bed;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bed
{
    public class GetPagedBedsHandler : IRequestHandler<GetPagedBedsQuery, (IEnumerable<BedListDto> Beds, int TotalCount)>
    {
        private readonly IMapper _mapper;
        private readonly IBedRepository _repository;

        public GetPagedBedsHandler(IMapper mapper, IBedRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<(IEnumerable<BedListDto> Beds, int TotalCount)> Handle(GetPagedBedsQuery request, CancellationToken cancellationToken)
        {
            var (beds, totalCount) = await _repository.GetPagedBedsAsync(request.PageNumber, request.PageSize);
            return (beds, totalCount);
        }
    }
}
