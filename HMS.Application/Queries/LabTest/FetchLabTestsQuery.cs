using HMS.Application.DTO.LabTest;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.LabTest
{
    public class FetchLabTestsQuery : IRequest<IEnumerable<FetchLabTestsDto>>
    {
    }
}
