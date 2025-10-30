using HMS.Application.Commands.Ward;
using HMS.Application.DTO.Ward;
using HMS.Application.Queries.Ward;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class WardController : Controller
    {
        private readonly IMediator _mediator;
        public WardController(IMediator mediator) { 
            _mediator = mediator;
        }
        public IActionResult Ward()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Assignment()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWardDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateWardCommand(dto);
            var newWardId = await _mediator.Send(command);

            if (newWardId > 0)
                return Json(new { success = true, message = "Ward added successfully.", wardId = newWardId });

            return Json(new { success = false, message = "Failed to add ward." });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllWardsQuery();
            var wards = await _mediator.Send(query);

            if (wards == null)
            {
                return Json(new { success = false, message = "No wards found." });
            }

            return Json(new { success = true, data = wards });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWard([FromBody] UpdateWardCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(new { message = "Invalid ward data." });

                var result = await _mediator.Send(command);

                if (result > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Ward updated successfully.",
                        rowsAffected = result
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "No ward found with the given ID or no changes detected."
                });
            }
            catch (ValidationException ex)
            {
                // Handle FluentValidation errors
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors = ex.Errors
                });
            }
            catch (Exception ex)
            {
                // Handle all other exceptions
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while updating the ward.",
                    details = ex.Message
                });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteWard([FromBody] DeleteWardCommand command)
        {
            try
            {
                if (command == null || command.Id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid Ward ID."
                    });
                }

                var result = await _mediator.Send(command);

                if (result > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Ward deleted successfully.",
                        rowsAffected = result
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "No ward found with the specified ID."
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors = ex.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while deleting the ward.",
                    details = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AssignDoctorToWard([FromBody] AssignDoctorWardDto dto)
        {
            try
            {
                if (dto == null || dto.DoctorId <= 0 || dto.WardId <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid input data." });
                }

                // Send command to MediatR
                var command = new AssignDoctorWardCommand(dto);
                var result = await _mediator.Send(command);

                if (result.IsSuccess == false)
                {
                    return StatusCode(500, new { success = false, message = "Unexpected error occurred." });
                }

                // Return JSON response for frontend AJAX
                return Json(new
                {
                    success = result.IsSuccess,
                    message = result.Message
                });
            } catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    message = e.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctorWardAssignments()
        {
            try
            {
                var result = await _mediator.Send(new GetAllDoctorWardAssignmentsQuery());

                if (result == null || result.Count() == 0)
                    return Json(new { success = false, message = "No doctor–ward assignments found." });

                return Json(new { success = true, data = result });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    message = e.Message
                });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> UnassignDoctor(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid assignment ID." });

                var result = await _mediator.Send(new DeleteDoctorWardAssignmentCommand(id));

                if (result.Success == false)
                    return Ok(new { success = true, message = "Doctor unassigned successfully." });
                else
                    return NotFound(new { success = false, message = "Assignment not found or already removed." });
            }
            catch (Exception ex)
            {
                // Optional: log error here if you have ILogger injected
                return StatusCode(500, new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

    }
}
