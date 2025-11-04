using HMS.Application.Commands.Bill;
using HMS.Application.DTO.Bill;
using HMS.Application.Queries.Bill;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class BillController : Controller
    {
        private readonly IMediator _mediator;
        public BillController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBill([FromBody] AddBillDto billDto)
        {
            if (billDto == null)
                return BadRequest("Invalid bill data.");

            try
            {
                // Create and send the MediatR command
                var command = new AddBillCommand(billDto);
                var newBillId = await _mediator.Send(command);

                if (newBillId > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Bill added successfully.",
                        billId = newBillId
                    });
                }

                return BadRequest("Failed to add bill. Please try again.");
            }
            catch (Exception ex)
            {
                // Log ex here if you have a logging service
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetBillsByPatientId(int patientId)
        {
            if (patientId <= 0)
                return BadRequest(new { success = false, message = "Invalid Patient ID." });

            try
            {
                var query = new GetBillsByPatientIdQuery(patientId);
                var bills = await _mediator.Send(query);

                if (bills == null)
                    return NotFound(new { success = false, message = "No bills found for this patient." });

                return Ok(new
                {
                    success = true,
                    data = bills
                });
            }
            catch (Exception ex)
            {
                // optional: inject and use a logger here
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching patient bills.",
                    details = ex.Message
                });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBill([FromBody] EditBillDto dto)
        {
            if (dto == null)
                return BadRequest(new { success = false, message = "Invalid data received." });

            try
            {
                var command = new EditBillCommand(dto);
                var result = await _mediator.Send(command);

                if (result > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Bill updated successfully.",
                        affectedRows = result
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "No records were updated. Please verify the Bill ID."
                    });
                }
            }
            catch (Exception ex)
            {
                // Log ex if you have a logging system (Serilog/NLog)
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBill(int id)
        {
            try
            {
                // Create the command (assuming you'll define DeleteBillCommand later)
                var command = new DeleteBillCommand(id);

                // Send command to handler via MediatR
                var result = await _mediator.Send(command);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Bill deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "No records deleted. Bill ID may not exist." });
                }
            }
            catch (Exception ex)
            {
                // Log error (if logger exists) and return friendly message
                return Json(new
                {
                    success = false,
                    message = $"Error deleting bill: {ex.Message}"
                });
            }
        }
    }
}
