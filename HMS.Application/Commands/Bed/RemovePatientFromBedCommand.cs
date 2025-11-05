using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bed
{
    public class RemovePatientFromBedCommand : IRequest<bool>
    {
        public int PatientBedWardId { get; set; }
    }
}
