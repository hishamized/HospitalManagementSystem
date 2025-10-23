using HMS.Application.Dto.Doctor;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Doctor
{
    public class GetDoctorsQuery : IRequest<IEnumerable<GetDoctorsDto>>
    {
    }
}
