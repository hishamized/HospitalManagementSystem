using MediatR;

namespace HMS.Application.Commands.DoctorDocument
{
    public class DeleteDoctorDocumentCommand : IRequest<bool>
    {
        public int DocumentId { get; set; }
        public string? FilePath { get; set; }

        public DeleteDoctorDocumentCommand(int documentId, string? filePath)
        {
            DocumentId = documentId;
            FilePath = filePath;
        }
    }
}
