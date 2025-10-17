using MediatR;
using HMS.Application.DTO.Allergy;

namespace HMS.Application.Commands.Allergy
{
    // The command carries the DTO for editing
    public class EditAllergyCommand : IRequest<bool>
    {
        public EditAllergyDto Dto { get; set; }

        public EditAllergyCommand(EditAllergyDto dto)
        {
            Dto = dto;
        }
    }

  
}
