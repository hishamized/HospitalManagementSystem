using MediatR;

namespace HMS.Application.Commands.Allergy
{
    public class DeleteAllergyCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteAllergyCommand(int id)
        {
            Id = id;
        }
    }
}
