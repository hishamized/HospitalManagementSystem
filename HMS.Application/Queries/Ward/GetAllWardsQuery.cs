using HMS.Application.DTO.Ward;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Ward
{
    public class GetAllWardsQuery : IRequest<IEnumerable<WardDto>>
    {
    }
}
