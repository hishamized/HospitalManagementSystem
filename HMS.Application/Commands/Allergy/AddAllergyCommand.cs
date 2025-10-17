using HMS.Application.DTO.Allergy;
using MediatR;

namespace HMS.Application.Commands.Allergy
{
    public class AddAllergyCommand : IRequest<int>  // Returns newly created Allergy Id
    {
        public AddAllergyDto Allergy { get; set; }

        public AddAllergyCommand(AddAllergyDto allergy)
        {
            Allergy = allergy;
        }
    }
}
