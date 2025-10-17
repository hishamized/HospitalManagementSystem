using MediatR;

namespace HMS.Application.Commands.MedicalHistory
{
    public class DeleteMedicalHistoryCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteMedicalHistoryCommand(int id)
        {
            Id = id;
        }
    }
}
