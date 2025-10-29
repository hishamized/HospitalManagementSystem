using AutoMapper;
using HMS.Application.Commands.DoctorDocument;
using HMS.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorDocument
{
    public class EditDoctorDocumentHandler : IRequestHandler<EditDoctorDocumentCommand, bool>
    {
        private readonly IDoctorDocumentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public EditDoctorDocumentHandler(
            IDoctorDocumentRepository repository,
            IMapper mapper,
            IWebHostEnvironment env)
        {
            _repository = repository;
            _mapper = mapper;
            _env = env;
        }

        public async Task<bool> Handle(EditDoctorDocumentCommand request, CancellationToken cancellationToken)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "documents", "doctors");
            Directory.CreateDirectory(uploadsFolder);

            // Convert relative old path ("/uploads/documents/doctors/xyz.png") to absolute
            string oldFilePath = string.IsNullOrWhiteSpace(request.OldFilePath)
                ? string.Empty
                : Path.Combine(_env.WebRootPath, request.OldFilePath.TrimStart('/', '\\'));

            string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
            string newFilePath = Path.Combine(uploadsFolder, newFileName);

            try
            {
                // 1️⃣ Delete old file safely
                if (File.Exists(oldFilePath)) {
                    Console.WriteLine($"Deleting: {oldFilePath}");
                    File.Delete(oldFilePath);
                }
           

                // 2️⃣ Upload new file
                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream, cancellationToken);
                }

                // 3️⃣ Update DTO with new metadata
                request.Dto.FileName = newFileName;
                request.Dto.FilePath = "/" + Path.Combine("uploads", "documents", "doctors", newFileName)
                                                .Replace("\\", "/");

                request.Dto.FileType = Path.GetExtension(request.File.FileName).TrimStart('.').ToUpper();
                request.Dto.FileSize = request.File.Length;
                request.Dto.UploadedBy = request.UploadedBy;

                // 4️⃣ Update DB
                bool isUpdated = await _repository.UpdateDoctorDocumentAsync(request.Dto);

                // 5️⃣ Rollback if DB fails
                if (!isUpdated)
                {
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                // Rollback if unexpected error
                if (File.Exists(newFilePath))
                    File.Delete(newFilePath);

                Console.WriteLine($"Error updating document: {ex.Message}");
                return false;
            }
        }

    }
}
