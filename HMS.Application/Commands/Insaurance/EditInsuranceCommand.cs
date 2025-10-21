using MediatR;
using HMS.Application.DTO.Insurance;

namespace HMS.Application.Commands.Insurance
{
    // Command that carries the EditInsuranceDto
    public class EditInsuranceCommand : IRequest<bool>
    {
        public EditInsuranceDto Dto { get; set; }

        public EditInsuranceCommand(EditInsuranceDto dto)
        {
            Dto = dto;
        }
    }
}
