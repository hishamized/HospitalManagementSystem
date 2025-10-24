using FluentValidation;
using HMS.Application.Commands.Appointment;
using HMS.Application.DTO.Appointment;
using HMS.Application.Features.Appointments.Commands;
using HMS.Application.Queries.Appointment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IMediator _mediator;

        public AppointmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Appointment()
        {
            return View(); // will load the appointment.cshtml view with AG Grid
        }

        // POST: Appointments/Add
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddAppointmentDTO appointmentDto)
        {
            try
            {
                // Send command to handler
                var command = new AddAppointmentCommand(appointmentDto);
                var newAppointmentId = await _mediator.Send(command);

                return Json(new
                {
                    success = true,
                    message = "Appointment booked successfully.",
                    id = newAppointmentId
                });
            }
            catch (ValidationException ex)
            {
                // Return FluentValidation errors
                return Json(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
            catch (Exception ex)
            {
                // Return general exception errors
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Send the query through MediatR
                var appointments = await _mediator.Send(new GetAllAppointmentsQuery());

                return Json(new
                {
                    success = true,
                    data = appointments
                });
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching appointments.",
                    details = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reschedule([FromBody] RescheduleAppointmentDto dto)
        {

            try
            {
                // Send command to Mediator
                var command = new RescheduleAppointmentCommand(dto);
                bool result = await _mediator.Send(command);

                if (result)
                    return Ok(new { success = true, message = "Appointment rescheduled successfully." });

                return BadRequest(new { success = false, message = "Failed to reschedule appointment." });


            }
            catch (FluentValidation.ValidationException ex)
            {
                // If your command has validator
                var errors = ex.Errors.Select(err => new { err.PropertyName, err.ErrorMessage });
                return BadRequest(new { success = false, message = "Validation failed.", errors });
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return StatusCode(500, new { success = false, message = "Server error occurred.", detail = ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid appointment ID." });

            var command = new DeleteAppointmentCommand(id);
            bool result = await _mediator.Send(command);

            if (result)
                return Ok(new { success = true, message = "Appointment deleted successfully." });

            return BadRequest(new { success = false, message = "Failed to delete appointment." });
        }
    }
}
