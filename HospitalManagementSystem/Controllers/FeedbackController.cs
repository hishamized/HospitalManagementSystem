using FluentValidation;
using HMS.Application.Commands.Feedback;
using HMS.Application.DTO.Feedback;
using HMS.Application.Queries.Feedback;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(IMediator mediator, ILogger<FeedbackController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetClientIP()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            return Ok(new { ip });
        }


        // Shows the feedback page (Feedback.cshtml will be created later)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View("Feedback");
        }

        // Accepts feedback submission and returns JSON result
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] CreateFeedbackDto dto)
        {
            try
            {
                var command = new CreateFeedbackCommand(dto);
                var newId = await _mediator.Send(command);

                return Json(new { success = true, feedbackId = newId });
            }
            catch (ValidationException vex)
            {
                _logger?.LogWarning(vex, "Feedback validation failed.");
                var errors = vex.Errors?.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>();
                return BadRequest(new { success = false, errors });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error while adding feedback.");
                return StatusCode(500, new { success = false, message = "An error occurred while submitting feedback." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                var query = new GetAllFeedbacksQuery();
                var result = await _mediator.Send(query);

                return Json(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching feedbacks.");

                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while fetching feedbacks.",
                    error = ex.Message
                });
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid feedback ID." });
                }

                var result = await _mediator.Send(new DeleteFeedbackCommand(id));

                if (result > 0)
                {
                    return Ok(new { success = true, message = "Feedback deleted successfully." });
                }
                else
                {
                    return NotFound(new { success = false, message = "Feedback not found or already deleted." });
                }
            }
            catch (Exception ex)
            {
                // Log error details (you can inject ILogger if needed)
                Console.WriteLine($"Error deleting feedback ID {id}: {ex.Message}");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while deleting feedback.",
                    error = ex.Message
                });
            }
        }
    }
}
