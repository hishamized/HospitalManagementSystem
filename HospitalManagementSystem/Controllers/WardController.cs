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

    }
}
