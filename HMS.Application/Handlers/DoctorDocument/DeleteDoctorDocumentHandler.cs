using HMS.Application.Commands.DoctorDocument;
using HMS.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorDocument
{
    public class DeleteDoctorDocumentHandler : IRequestHandler<DeleteDoctorDocumentCommand, bool>
    {
        private readonly IDoctorDocumentRepository _repository;
        private readonly IWebHostEnvironment _env;

        public DeleteDoctorDocumentHandler(IDoctorDocumentRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<bool> Handle(DeleteDoctorDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🗑️ 1️⃣ Delete record from database
                bool isDeleted = await _repository.DeleteDoctorDocumentAsync(request.DocumentId);

                if (!isDeleted)
                    return false;

                // 📁 2️⃣ Delete file from server (optional)
                if (!string.IsNullOrWhiteSpace(request.FilePath))
                {
                    string fullPath = Path.Combine(_env.WebRootPath, request.FilePath.TrimStart('/', '\\'));

                    if (File.Exists(fullPath))
                    {
                        Console.WriteLine($"Deleting file: {fullPath}");
                        File.Delete(fullPath);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting doctor document: {ex.Message}");
                return false;
            }
        }
    }
}
