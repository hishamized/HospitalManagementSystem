using HMS.Application.DTO.DoctorPortal;
using MediatR;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.DoctorPortal
{
    public class FetchValidTestsQuery : IRequest<IEnumerable<FetchValidTestsDto>>
    {
        // Add any properties or constructors as needed
    }
}
