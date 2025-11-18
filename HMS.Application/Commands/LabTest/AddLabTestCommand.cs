using HMS.Application.DTO.LabTest;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.LabTest
{
    public class AddLabTestCommand : IRequest<long>
    {
        public readonly AddLabTestDto dto;
        public AddLabTestCommand(AddLabTestDto _dto) {
            dto = _dto;
        }
    }
}
