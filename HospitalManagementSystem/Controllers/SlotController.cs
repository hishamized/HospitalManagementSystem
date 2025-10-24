using HMS.Application.Commands.Slot;
using HMS.Application.DTO.Slot;
using HMS.Application.Queries.Slot;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace HMS.Web.Controllers
{
    [Authorize] 

    public class SlotController : Controller
    {
        private readonly IMediator _mediator;
        public SlotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Slot()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddSlotDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Collect all validation errors
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                        );

                    return Json(new { success = false, message = "Validation errors occurred.", errors });
                }

                // Send command to MediatR handler
                var command = new AddSlotCommand(dto);
                var result = await _mediator.Send(command);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Slot added successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to add slot." });
                }
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errorDict = ex.Errors
                    .GroupBy(e => e.PropertyName.Split('.').Last()) // "Slot.LeavingTime" → "LeavingTime"
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().ErrorMessage
                    );

                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors = errorDict
                });
            }

            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return Json(new { success = false, message = "An error occurred.", errors = new[] { ex.Message } });
            }
        }

        // GET: Retrieve all slots for AG Grid
        [HttpGet]
        public async Task<IActionResult> GetAllSlots()
        {
            try
            {
                var query = new GetAllSlotsQuery();
                var slots = await _mediator.Send(query);

                return Json(new { success = true, data = slots });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while retrieving slots.", errors = new[] { ex.Message } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditSlot([FromBody] EditSlotDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return Json(new { success = false, message = "Validation failed.", errors });
            }

            try
            {
                var rowsAffected = await _mediator.Send(new EditSlotCommand(dto));

                if (rowsAffected > 0)
                {
                    return Json(new { success = true, message = "Slot updated successfully." });
                }

                return Json(new { success = false, message = "No changes were made or slot not found." });
            }
            catch (FluentValidation.ValidationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors = ex.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the slot.",
                    errors = new[] { ex.Message }
                });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return Json(new { success = false, message = "Invalid slot ID." });

                bool deleted = await _mediator.Send(new DeleteSlotCommand(id));

                if (!deleted)
                    return Json(new { success = false, message = "Slot not found." });

                return Json(new { success = true, message = "Slot deleted successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging
                // _logger.LogError(ex, "Error deleting slot");

                return Json(new { success = false, message = "An unexpected error occurred.", detail = ex.Message });
            }
        }
        


    }
}
