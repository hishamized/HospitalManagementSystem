using HMS.Application.Commands.Role;
using HMS.Application.Dto.Role;
using HMS.Application.Queries.Role;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System;
using System.Threading.Tasks;

namespace HMS.Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Role()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var query = new GetRolesQuery();
                var roles = await _mediator.Send(query);

                return Json(new
                {
                    success = true,
                    message = "Roles retrieved successfully.",
                    data = roles
                });
            }
            catch (Exception ex)
            {
                // Log exception (if you have centralized logging)
                Console.WriteLine(ex.Message);

                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching roles.",
                    errors = new[] { ex.Message }
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddRoleDto roleDto)
        {
            try
            {
                if (roleDto == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid data submitted."
                    });
                }

                var command = new AddRoleCommand(roleDto);

                var result = await _mediator.Send(command);

                // Return structured JSON for AJAX
                return Json(new
                {
                    success = result > 0,
                    message = result > 0
                        ? "Role added successfully."
                        : "Failed to add role. Please try again."
                });
            }
            catch (ValidationException ex)
            {
                // Handle FluentValidation errors
                var errorMessages = ex.Errors
                    .Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
                    .ToList();

                // Log validation errors to console for debugging
                Console.WriteLine("Validation Errors: " + string.Join(", ", errorMessages.Select(e => e.error)));

                return Json(new
                {
                    success = false,
                    message = "Validation failed. Please correct the errors.",
                    errors = errorMessages
                });
            }
            catch (Exception ex)
            {
                // Log general exception
                Console.WriteLine($"[AddRole Error]: {ex.Message}");

                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while adding the role. Please try again later." + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return Json(new { success = false, message = "Invalid role ID." });

            // Send the command to the handler
            var result = await _mediator.Send(new TheDeleteRoleCommand(id));

            // Return JSON for AJAX
            if (result.Success)
            {
                return Json(new
                {
                    success = true,
                    message = result.Message
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditRoleDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return Json(new { success = false, message = "Invalid role data provided." });
                }

                // Send command to MediatR
                var result = await _mediator.Send(new EditRoleCommand(dto));

                // If repository returns 0 rows affected, it means no update happened
                if (result <= 0)
                {
                    return Json(new { success = false, message = "No changes were made or role not found." });
                }

                return Json(new { success = true, message = "Role updated successfully." });
            }
            catch (FluentValidation.ValidationException ex)
            {
                // Handle validation errors coming from FluentValidation
                var errors = ex.Errors
                    .Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
                    .ToList();

                return Json(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors
                });
            }
            catch (Exception ex)
            {
                // Handle unexpected server errors
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while updating the role.",
                    details = ex.Message
                });
            }
        }
    }
}
