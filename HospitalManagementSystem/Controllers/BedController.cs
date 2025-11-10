using HMS.Application.Commands.Bed;
using HMS.Application.DTO.Bed;
using HMS.Application.Queries.Bed;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    public class BedController : Controller
    {
        private readonly IMediator _mediator;

        public BedController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public IActionResult Index()
        {
            return View("~/Views/Bed/Bed.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBed([FromBody] AddBedDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });

                var result = await _mediator.Send(new AddBedCommand(dto));

                if (result > 0)
                    return Ok(new { success = true, message = "Bed added successfully", bedId = result });

                return BadRequest(new { success = false, message = "Unable to add bed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedBeds(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // ✅ Handle fetch-all scenario
                if (pageSize == 0)
                {
                    var allBedsQuery = new GetPagedBedsQuery(1, 0);
                    var (allBeds, totalCount) = await _mediator.Send(allBedsQuery);

                    return Json(new
                    {
                        success = true,
                        message = "Fetched all records.",
                        data = allBeds,
                        totalCount
                    });
                }

                // ✅ Basic input validation
                if (pageNumber <= 0 || pageSize < 0)
                    return BadRequest(new { message = "Invalid pagination parameters." });

                // ✅ Regular pagination flow
                var query = new GetPagedBedsQuery(pageNumber, pageSize);
                var (beds, totalCountPaged) = await _mediator.Send(query);

                return Json(new
                {
                    success = true,
                    data = beds,
                    totalCount = totalCountPaged
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching beds.",
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditBed([FromBody] EditBedDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });

                var command = new EditBedCommand(dto);
                var result = await _mediator.Send(command);

                if (result > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Bed updated successfully."
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to update bed. No rows affected."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while updating the bed.",
                    error = ex.Message
                });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBed(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid Bed ID." });

                var result = await _mediator.Send(new DeleteBedCommand(id));

                if (result > 0)
                    return Ok(new { success = true, message = "Bed deleted successfully." });

                return NotFound(new { success = false, message = "Bed not found or could not be deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while deleting the bed.",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByWardId(int wardId)
        {
            try
            {
                if (wardId <= 0)
                    return BadRequest("Invalid Ward Id.");

                var result = await _mediator.Send(new GetBedsByWardIdCommand(wardId));

                if (result == null)
                    return NotFound("No beds found for the selected ward.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception here if you use a logging framework
                return StatusCode(500, $"An error occurred while fetching beds: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AllotBed([FromBody] AllotBedDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Invalid request data.");

                if (dto.PatientId <= 0 || dto.WardId <= 0 || dto.BedId <= 0)
                    return BadRequest("Patient, Ward, and Bed are required fields.");

                var result = await _mediator.Send(new AllotBedCommand(dto));

                if (result != 1)
                    return StatusCode(500, "Failed to allot bed. Please try again.");

                return Ok("Bed successfully allotted to patient.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while allotting bed: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckBed(int patientId)
        {
            try
            {
                var command = new CheckBedCommand(patientId);
                var result = await _mediator.Send(command);

                if (result == null)
                    return Json(new { success = false, message = "No data found." });

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemovePatientFromBed([FromBody] RemovePatientFromBedCommand command)
        {
            try
            {
                if (command == null || command.PatientBedWardId <= 0)
                    return BadRequest(new { success = false, message = "Invalid request." });

                var result = await _mediator.Send(command);

                if (result)
                    return Ok(new { success = true, message = "Patient successfully removed from bed." });
                else
                    return BadRequest(new { success = false, message = "Failed to remove patient from bed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
