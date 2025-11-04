using HMS.Application.DTO.Bed;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Bed
{
    public class GetPagedBedsQuery : IRequest<(IEnumerable<BedListDto> Beds, int TotalCount)>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetPagedBedsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
