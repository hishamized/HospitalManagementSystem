using HMS.Application.Commands.Department;
using HMS.Application.DTO.Department;
using HMS.Application.Queries.Department;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;


namespace HMS.Web.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Department()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDepartmentsDto dto)
        {
            if (dto == null)
                return BadRequest(new { success = false, message = "Invalid department data." });

            try
            {
                // Send command to MediatR
                var command = new AddDepartmentCommand(dto);
                var newDepartmentId = await _mediator.Send(command);

                if (newDepartmentId > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Department added successfully.",
                        departmentId = newDepartmentId
                    });
                }

                return BadRequest(new { success = false, message = "Failed to add department." });
            }
            catch (FluentValidation.ValidationException ex)
            {
                // Return validation errors to the UI
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors = ex.Errors
                });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error adding department");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    detail = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = new GetAllDepartmentsQuery();
                var departments = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    data = departments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while fetching departments.",
                    detail = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditDepartmentDto dto)
        {
            try
            {
                var result = await _mediator.Send(new EditDepartmentCommand(dto));

                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        message = result.Message
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
            catch (ValidationException ex)
            {
                // Automatically handled FluentValidation errors
                return Json(new
                {
                    success = false,
                    errors = ex.Errors.Select(e => new
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    })
                });
            }
            catch (Exception ex)
            {
                // Generic runtime or DB-level error
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while updating the department.",
                    details = ex.Message // helpful for console logs
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            try
            {
                var rowsAffected = await _mediator.Send(new DeleteDepartmentCommand(id));

                if (rowsAffected > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Department deleted successfully."
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Department not found or could not be deleted."
                    });
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while deleting the department.",
                    details = ex.Message
                });
            }
        }
    }
}
