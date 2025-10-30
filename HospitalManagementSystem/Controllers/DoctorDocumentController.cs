using HMS.Application.Commands.DoctorDocument;
using HMS.Application.DTO.DoctorDocument;
using HMS.Application.DTOs;
using HMS.Application.Queries.DoctorDocument;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using FluentValidation;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class DoctorDocumentController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;

        public DoctorDocumentController(IMediator mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _env = env;
        }

        public IActionResult Document()
        {
            return View();
        }

      
        public IActionResult ViewDocument() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile file, int doctorId)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "No file selected." });
            }

            try
            {
                // Folder: wwwroot/uploads/documents/doctors
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "documents", "doctors");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Generate unique file name
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Prepare DTO for database insertion
                var documentDto = new DoctorDocumentDto
                {
                    DoctorId = doctorId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/documents/doctors/{uniqueFileName}",
                    FileType = Path.GetExtension(file.FileName)?.TrimStart('.').ToUpper() ?? "UNKNOWN",
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    UploadedBy = User.Identity?.Name ?? "System",
                    IsActive = true
                };

                // Send MediatR command
                var command = new AddDoctorDocumentCommand(documentDto);
                var newDocumentId = await _mediator.Send(command);

                if (newDocumentId > 0)
                    return Json(new { success = true, message = "Document uploaded successfully.", id = newDocumentId });

                return Json(new { success = false, message = "Failed to save document metadata." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctorDocuments()
        {
            try
            {
                var result = await _mediator.Send(new GetAllDoctorDocumentsWithDoctorsQuery());

                return Json(new
                {
                    success = true,
                    data = result
                });
            }
            catch (System.Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error fetching documents: {ex.Message}"
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDocument(EditDoctorDocumentDto dto)
        {
            try
            {
                var file = Request.Form.Files.FirstOrDefault();
                if (file == null)
                    return BadRequest(new { success = false, message = "No file uploaded." });

                // ✅ Fetch OldFilePath manually from the form data
                var oldFilePath = Request.Form["OldFilePath"].ToString();

                var command = new EditDoctorDocumentCommand
                {
                    Dto = dto,
                    File = file,
                    OldFilePath = oldFilePath ,
                     UploadedBy = User.Identity?.Name ?? "System",
                };

                bool result = await _mediator.Send(command);

                if (result)
                    return Ok(new { success = true, message = "Document updated successfully." });
                else
                    return BadRequest(new { success = false, message = "Failed to update document." });
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage });
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while updating the document.",
                    details = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(int id, string? filePath)
        {
            try
            {
                var command = new DeleteDoctorDocumentCommand(id, filePath);
                bool result = await _mediator.Send(command);

                if (result)
                {
                    return Json(new { success = true, message = "Document deleted successfully." });
                }

                return Json(new { success = false, message = "Failed to delete document." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while deleting the document.",
                    details = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorDocumentsByDoctorId(int doctorId)
        {
            if (doctorId <= 0)
                return BadRequest("Invalid Doctor ID");

            var documents = await _mediator.Send(new GetDoctorDocumentsByDoctorIdCommand(doctorId));

            if (documents == null)
                return NotFound("No documents found for this doctor.");

            return Json(documents);
        }

    }
}
