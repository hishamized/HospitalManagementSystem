using HMS.Application.DTO.Bed;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bed
{
    public class CheckBedCommand : IRequest<CheckBedDto>
    {
        public int PatientId { get; set; }

        public CheckBedCommand(int patientId)
        {
            PatientId = patientId;
        }
    }
}
