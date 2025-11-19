using MediatR;
using HMS.Application.DTO.LabTest;

namespace HMS.Application.Commands.LabTest
{
    public class EditLabTestCommand : IRequest<bool>
    {
        public EditLabTestDto Dto;
        public  EditLabTestCommand(EditLabTestDto dto) { 
            Dto = dto;
        }
    }
}
