using HMS.Application.Dto;
using HMS.Application.DTO.Doctor;
using HMS.Application.Features.Doctors.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IMediator _mediator;
        public DoctorController(IMediator Mediator) {
            _mediator = Mediator;
        }

        [HttpGet]
        public IActionResult Doctor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromForm] AddDoctorDto doctorDto)
        {
            try
            {
                var command = new AddDoctorCommand(doctorDto);
                int newDoctorId = await _mediator.Send(command);

                return Json(new { success = true, message = "Doctor added successfully!", id = newDoctorId });
            }
            catch (FluentValidation.ValidationException ex)
            {
                // Return concatenated validation errors
                var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

    }
}
