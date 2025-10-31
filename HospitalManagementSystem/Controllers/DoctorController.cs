using HMS.Application.Commands.Doctor;
using HMS.Application.Dto;
using HMS.Application.Dto.Doctor;
using HMS.Application.DTO.Doctor;
using HMS.Application.Features.Doctors.Commands;
using HMS.Application.Queries.Doctor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HMS.Web.Controllers
{
    [Authorize]
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

        [HttpGet]
        public IActionResult Availability()
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

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _mediator.Send(new GetDoctorsQuery());

                if (doctors == null)
                {
                    return NotFound(new { message = "No doctors found." });
                }

                return Ok(new
                {
                    success = true,
                    data = doctors
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    success = false,
                    message = "An unexpected error occurred while fetching doctor records. Please try again later."
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditDoctor([FromBody] EditDoctorDto dto)
        {
            if (dto == null)
                return BadRequest(new { success = false, message = "Doctor data is required." });

            try
            {
                var command = new EditDoctorCommand(dto);
                var result = await _mediator.Send(command);

                if (result)
                    return Ok(new { success = true, message = "Doctor updated successfully." });
                else
                    return NotFound(new { success = false, message = "Doctor not found or update failed." });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = "An error occurred while updating the doctor.", details = ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid doctor ID." });

            try
            {
                var command = new DeleteDoctorCommand(id);
                var result = await _mediator.Send(command);

                if (result)
                    return Ok(new { success = true, message = "Doctor deleted successfully." });
                else
                    return NotFound(new { success = false, message = "Doctor not found or already deleted." });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while deleting the doctor.",
                    details = ex.Message
                });
            }
        }
    }
}
