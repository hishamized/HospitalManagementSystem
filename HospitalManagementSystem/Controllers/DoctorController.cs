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

        [HttpGet]
        public async Task<IActionResult> GetDoctorsByPatient(int patientId)
        {
            if (patientId <= 0)
                return BadRequest("Invalid patient ID");

            try
            {
                IEnumerable<DoctorSelectDto> doctors = await _mediator.Send(new GetDoctorsByPatientQuery(patientId));
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return StatusCode(500, new { Message = "An error occurred while fetching doctors.", Details = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctorRound([FromForm] AddDoctorRoundDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new AddDoctorRoundCommand(dto);
                var newRoundId = await _mediator.Send(command);

                return Json(new
                {
                    success = true,
                    message = "Doctor round added successfully",
                    roundId = newRoundId
                });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging

                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetDoctorRoundsByPatient(int patientId)
        {
            if (patientId <= 0)
                return BadRequest(new { success = false, message = "Invalid patient ID." });

            try
            {
                var query = new GetDoctorRoundsByPatientQuery(patientId);
                IEnumerable<DoctorRoundHistoryDto> rounds = await _mediator.Send(query);

                return Json(new
                {
                    success = true,
                    data = rounds
                });
            }
            catch (Exception ex)
            {
                // Log exception if logging is configured
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorRound([FromForm] UpdateDoctorRoundDto doctorRound)
        {
            if (doctorRound == null)
                return Json(new { success = false, message = "Invalid request data." });

            try
            {
                // Send the command
                var command = new EditDoctorRoundCommand(doctorRound);
                var updatedRound = await _mediator.Send(command);

                if (updatedRound == null)
                    return Json(new { success = false, message = "Doctor round not found or update failed." });

                return Json(new { success = true, message = "Doctor round updated successfully.", data = updatedRound });
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // _logger.LogError(ex, "Error updating doctor round");
                return Json(new { success = false, message = "An error occurred while updating the doctor round.", error = ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteDoctorRound(int id)
        {
            try
            {
                var command = new DeleteDoctorRoundCommand(id);
                var result = await _mediator.Send(command);

                if (result)
                    return Ok(new { success = true, message = "Doctor round deleted successfully." });
                else
                    return BadRequest(new { success = false, message = "Doctor round not found or could not be deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the doctor round.", details = ex.Message });
            }
        }
    }
}
