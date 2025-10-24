using HMS.Application.DTO.Appointment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Appointment
{
    public class AddAppointmentCommand : IRequest<int>
    {
        public AddAppointmentDTO AppointmentDto { get; set; }

        public AddAppointmentCommand(AddAppointmentDTO dto)
        {
            AppointmentDto = dto;
        }
    }
}
