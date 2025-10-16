using MediatR;

namespace HMS.Application.Commands
{
    public class DeletePatientCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public DeletePatientCommand(int id) => Id = id;
    }
}
