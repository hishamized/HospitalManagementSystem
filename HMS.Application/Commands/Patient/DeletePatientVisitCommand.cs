using MediatR;

namespace HMS.Application.Commands.PatientVisit
{
    public class DeletePatientVisitCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public DeletePatientVisitCommand(int id) => Id = id;
    }
}
